namespace XMLAutoTagger.Domain.Enums
{
    /// <summary>
    /// Enumeration of communication events mapped directly to event_id in user_mgmt.communication_events table.
    /// </summary>
    public enum CommunicationEvent
    {
        /// <summary>
        /// Triggered when a new user account is created.
        /// </summary>
        UserCreated = 1,

        /// <summary>
        /// Triggered when an administrator resets a user's password.
        /// </summary>
        PasswordResetAdmin = 2,

        /// <summary>
        /// Triggered when a user changes their own password via self-service.
        /// </summary>
        PasswordChangedSelf = 3,

        /// <summary>
        /// Triggered when a user account is locked due to security policy or admin action.
        /// </summary>
        UserAccountLocked = 4
    }
}
