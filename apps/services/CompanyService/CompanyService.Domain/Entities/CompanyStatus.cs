// ─────────────────────────────────────────────────────────────────────────
// CompanyStatus.cs — Company Lifecycle Status Enum
//
// WHY REPLACE bool IsActive WITH AN ENUM?
//
//   bool IsActive approach (Week 1):
//     true  = active
//     false = inactive (covers BOTH pending setup AND archived)
//     Problem: You can't distinguish "never finished wizard" from "closed down"
//
//   CompanyStatus enum approach (Week 2):
//     Pending  = registered but wizard not yet completed
//     Active   = wizard complete, fully operational
//     Archived = company closed or suspended
//
//   This gives the UI much more information:
//     Pending  → show "Complete your setup" banner
//     Active   → show full dashboard
//     Archived → show "Account suspended" screen
//
// EXPLICIT NUMERIC VALUES (= 1, 2, 3):
//   Always use explicit numbers for enums stored in a database!
//   If you reorder the enum without explicit values, EF Core would
//   interpret all existing DB rows with wrong statuses.
// ─────────────────────────────────────────────────────────────────────────

namespace CompanyService.Domain.Entities;

/// <summary>
/// Represents the current lifecycle stage of a Company.
///
/// Flow:
///   Register → [Pending] → CompleteSetup() → [Active] → Archive() → [Archived]
///
/// Soft Delete:
///   Delete() can be called from ANY status. Sets IsDeleted = true.
///   Deleted companies are hidden from normal queries (EF query filter).
/// </summary>
public enum CompanyStatus
{
    /// <summary>
    /// Company has registered but not yet completed the onboarding wizard.
    /// Limited access — cannot use payroll, employees, or jobs features.
    /// This is the DEFAULT status for all new registrations.
    /// </summary>
    Pending = 1,

    /// <summary>
    /// Onboarding wizard completed. Company is fully operational.
    /// All features unlocked. Billing is active.
    /// </summary>
    Active = 2,

    /// <summary>
    /// Company has been suspended, cancelled, or closed.
    /// Data is retained for legal/audit purposes.
    /// Login is blocked until reactivated by an admin.
    /// </summary>
    Archived = 3
}