// ─────────────────────────────────────────────────────────────────────
// init.js — MongoDB Initialization Script
//
// Runs when the MongoDB container starts for the first time.
// Creates the CandidateService database, collection, and indexes.
// ─────────────────────────────────────────────────────────────────────

// Switch to (or create) the 'staffpro_candidates' database
// In MongoDB, databases are created automatically when you first write to them.
// getSiblingDB() switches the 'db' variable to point to our database.
db = db.getSiblingDB('staffpro_candidates');

// ── Create the 'candidates' collection with validation rules
// A collection is MongoDB's equivalent of a SQL table.
// The validator enforces that documents must have certain fields.
db.createCollection('candidates', {
  validator: {
    $jsonSchema: {
      bsonType: 'object', // Documents must be objects (not arrays, etc.)
      required: ['firstName', 'lastName', 'email', 'createdAt'], // These fields are mandatory
      properties: {
        firstName: {
          bsonType: 'string',
          description: 'First name is required and must be a string',
        },
        lastName: {
          bsonType: 'string',
          description: 'Last name is required and must be a string',
        },
        email: {
          bsonType: 'string',
          description: 'Email is required and must be a string',
        },
        createdAt: {
          bsonType: 'date',
          description: 'Creation date is required and must be a Date type',
        },
      },
    },
  },
});

// ── Create indexes for performance
// Without indexes, MongoDB reads EVERY document to find a match (very slow with big data).
// With indexes, MongoDB can jump directly to the right document (like a book's index).

// Unique index on email: ensures no two candidates have the same email address
// { unique: true } means MongoDB will reject duplicate emails
db.candidates.createIndex(
  { email: 1 }, // 1 = ascending order
  { unique: true, name: 'IX_candidates_email' }
);

// Index on companyId: makes queries like "find all candidates for company X" fast
db.candidates.createIndex({ companyId: 1 }, { name: 'IX_candidates_companyId' });

// Text search index: enables full-text search across name and email fields
// This lets you search for "john" and find "John Smith" or "Johnny@email.com"
db.candidates.createIndex(
  { firstName: 'text', lastName: 'text', email: 'text' },
  { name: 'IX_candidates_text_search' }
);

print('MongoDB init complete: staffpro_candidates database and candidates collection ready.');
