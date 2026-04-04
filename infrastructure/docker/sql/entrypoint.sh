#!/bin/bash
# ─────────────────────────────────────────────────────────────────
# entrypoint.sh — SQL Server Docker Entrypoint
#
# Problem: SQL Server doesn't support /docker-entrypoint-initdb.d
# Solution: Start SQL Server in background, wait for it to be ready,
#           run init.sql, then keep the container alive.
# ─────────────────────────────────────────────────────────────────

echo ">>> Starting SQL Server in background..."
/opt/mssql/bin/sqlservr &
SQL_PID=$!

echo ">>> Waiting for SQL Server to be ready (up to 90 seconds)..."
for i in $(seq 1 18); do
  sleep 5
  /opt/mssql-tools18/bin/sqlcmd \
    -S localhost \
    -U sa \
    -P "$SA_PASSWORD" \
    -Q "SELECT 1" \
    -b -C -No \
    > /dev/null 2>&1

  if [ $? -eq 0 ]; then
    echo ">>> SQL Server is ready! Running init.sql..."
    /opt/mssql-tools18/bin/sqlcmd \
      -S localhost \
      -U sa \
      -P "$SA_PASSWORD" \
      -C \
      -i /init/init.sql
    echo ">>> init.sql complete."
    break
  fi

  echo ">>> Attempt $i/18 — SQL Server not ready yet, waiting 5s..."
done

# Hand control back to SQL Server process (keeps container alive)
wait $SQL_PID