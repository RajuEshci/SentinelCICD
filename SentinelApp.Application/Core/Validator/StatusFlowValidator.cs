using System;
using System.Collections.Generic;

namespace SentinelApp.Application.Core.Validator
{
    public class StatusFlowValidator
    {
        /// <summary>
        /// Validates status transition based on business rules
        /// </summary>
        /// <param name="currentStatus">Existing status from trade detail</param>
        /// <param name="newStatus">New status to be set</param>
        /// <returns>Tuple with (isValid, errorMessage)</returns>
        public static (bool isValid, string errorMessage) ValidateStatusTransition(
            string currentStatus,
            string newStatus)
        {
            // Check for null or empty values
            if (string.IsNullOrWhiteSpace(currentStatus))
                return (false, "Current status cannot be empty.");

            if (string.IsNullOrWhiteSpace(newStatus))
                return (false, "New status cannot be empty.");

            // If status is the same, check if same-status transitions are allowed
            //if (string.Equals(currentStatus, newStatus, StringComparison.OrdinalIgnoreCase))
            //{
            //    // Only certain statuses allow same-status transitions
            //    if (string.Equals(currentStatus, "Input", StringComparison.OrdinalIgnoreCase) ||
            //        string.Equals(currentStatus, "Sent To Broker", StringComparison.OrdinalIgnoreCase))
            //    {
            //        return (true, string.Empty);
            //    }

            //    return (false, $"Status '{currentStatus}' cannot remain unchanged. The trade must progress to a new status.");
            //}

            // Define valid transitions using case-insensitive comparison
            var validTransitions = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
            {
                // Input -> Approved OR Approved with Update Or Reject (Input can stay as Input for updates)
                ["Input"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Approved",
                    "Approve With Update",
                    "Rejected",
                    "Input"
                },

                // Approved -> SentToBroker or Reject
                ["Approved"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Sent to Broker",
                    "Rejected"
                },

                // Approve With Update -> SentToBroker or Reject
                ["Approve With Update"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Sent to Broker",
                    "Rejected"
                },

                // SentToBroker -> Filled OR SentToBrokerUpdate (SentToBroker can stay as SentToBroker for updates)
                ["Sent to Broker"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Filled",
                    "Update"
                },

                // Filled -> SendToCustodian OR Delivered OR Settled (For Manual)
                ["Filled"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Send to Custodian",
                    "Delivered",
                    "Settled"
                },

                // SendToCustodian -> Delivered OR Settled (For Manual)
                ["Send to Custodian"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Delivered",
                    "Settled"
                },

                // Delivered -> Settled (For Manual)
                ["Delivered"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Settled"
                },

                // Settled -> SendToCustodian OR Delivered (For Manual)
                ["Settled"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "Send to Custodian",
                    "Delivered"
                },

                // Completed is terminal status (no transitions allowed)
                ["Completed"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase),

                // Reject is terminal status (no transitions allowed)
                ["Rejected"] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            };

            // Check if current status exists in our rules
            if (!validTransitions.ContainsKey(currentStatus))
            {
                return (false, $"Invalid current status '{currentStatus}'. Please contact system administrator.");
            }

            // Check if transition is allowed
            if (!validTransitions[currentStatus].Contains(newStatus))
            {
                // Generate specific error messages based on the current status
                return GetErrorMessage(currentStatus, newStatus);
            }

            return (true, string.Empty);
        }

        private static (bool isValid, string errorMessage) GetErrorMessage(
            string currentStatus,
            string newStatus)
        {
            // Custom error messages for specific scenarios
            var errorMessages = new Dictionary<string, Func<string>>(StringComparer.OrdinalIgnoreCase)
            {
                //["Completed"] = () => $"Trade is already completed and cannot be modified. Current status: '{currentStatus}'.",

                //["Rejected"] = () => $"Trade has been rejected and cannot be modified. Current status: '{currentStatus}'.",

                //["Approved"] = () => $"Approved trades can only be sent to broker or rejected.",

                //["Approve With Update"] = () => $"Trades with 'Approve With Update' status can only be sent to broker or rejected.",

                //["Sent to Broker"] = () => IsRollbackAttempt(currentStatus, newStatus)
                //    ? $"Cannot rollback from '{currentStatus}' to '{newStatus}'. Once sent to broker, trades can only be filled or updated."
                //    : $"Trades sent to broker can only be filled or updated.",

                //["Filled"] = () => IsRollbackAttempt(currentStatus, newStatus)
                //    ? $"Cannot rollback from '{currentStatus}' to '{newStatus}'. Filled trades must proceed to custody or settlement process."
                //    : $"Filled trades must proceed to custody or settlement and can not be update.",

                //["Send to Custodian"] = () => IsRollbackAttempt(currentStatus, newStatus)
                //    ? $"Cannot rollback from '{currentStatus}' to '{newStatus}'. Trades sent to custodian must be delivered or settled."
                //    : $"Trades sent to custodian can only be delivered or settled.",

                //["Delivered"] = () => IsRollbackAttempt(currentStatus, newStatus)
                //    ? $"Cannot rollback from '{currentStatus}' to '{newStatus}'. Delivered trades must be settled."
                //    : $"Delivered trades can only be settled.",

                //["Settled"] = () => IsRollbackAttempt(currentStatus, newStatus)
                //    ? $"Cannot rollback from '{currentStatus}' to '{newStatus}'. Settled trades can only move between custody statuses."
                //    : $"Settled trades can only be sent to custodian or delivered.",

                //["Input"] = () => $"Input trades can only be approved, approved with updates, or rejected."

                 ["Completed"] = () => $"The order has already been processed. Kindly refresh the page and check again.",

                ["Rejected"] = () => $"The order has already been processed. Kindly refresh the page and check again.",

                ["Approved"] = () => $"The order has already been processed. Kindly refresh the page and check again.",

                ["Approve With Update"] = () => $"The order has already been processed. Kindly refresh the page and check again.",

                ["Sent to Broker"] = () => IsRollbackAttempt(currentStatus, newStatus)
                    ? $"The order has already been processed. Kindly refresh the page and check again."
                    : $"The order has already been processed. Kindly refresh the page and check again.",

                ["Filled"] = () => IsRollbackAttempt(currentStatus, newStatus)
                    ? $"The order has already been processed. Kindly refresh the page and check again."
                    : $"The order has already been processed. Kindly refresh the page and check again.",

                ["Send to Custodian"] = () => IsRollbackAttempt(currentStatus, newStatus)
                    ? $"The order has already been processed. Kindly refresh the page and check again."
                    : $"The order has already been processed. Kindly refresh the page and check again.",

                ["Delivered"] = () => IsRollbackAttempt(currentStatus, newStatus)
                    ? $"The order has already been processed. Kindly refresh the page and check again."
                    : $"The order has already been processed. Kindly refresh the page and check again.",

                ["Settled"] = () => IsRollbackAttempt(currentStatus, newStatus)
                    ? $"The order has already been processed. Kindly refresh the page and check again."
                    : $"The order has already been processed. Kindly refresh the page and check again.",

                ["Input"] = () => $"The order has already been processed. Kindly refresh the page and check again."
            };

            // Check if we have a specific error message for the current status
            if (errorMessages.ContainsKey(currentStatus))
            {
                return (false, errorMessages[currentStatus]());
            }

            // Default error message for unknown transitions
            return (false, $"Invalid status change from '{currentStatus}' to '{newStatus}'. Please check the workflow rules.");
        }

        private static bool IsRollbackAttempt(string currentStatus, string newStatus)
        {
            // Define the status hierarchy/flow order
            var statusFlowOrder = new List<string>
            {
                "Input",
                "Approved",
                "Approve With Update",
                "Sent to Broker",
                "Filled",
                "Send to Custodian",
                "Delivered",
                "Settled",
                "Completed"
            };

            int currentIndex = statusFlowOrder.FindIndex(s => string.Equals(s, currentStatus, StringComparison.OrdinalIgnoreCase));
            int newIndex = statusFlowOrder.FindIndex(s => string.Equals(s, newStatus, StringComparison.OrdinalIgnoreCase));

            // If either status not found in flow order, not a rollback
            if (currentIndex == -1 || newIndex == -1)
                return false;

            // Rollback attempt if trying to go to an earlier status in the flow
            return newIndex < currentIndex;
        }

        /// <summary>
        /// Helper method to get all allowed next statuses for a given current status
        /// </summary>
        public static List<string> GetAllowedNextStatuses(string currentStatus)
        {
            if (string.IsNullOrWhiteSpace(currentStatus))
                return new List<string>();

            var validTransitions = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                ["Input"] = new List<string> { "Approved", "Approve With Update", "Reject", "Input" },
                ["Approved"] = new List<string> { "Sent to Broker", "Reject" },
                ["Approve With Update"] = new List<string> { "Sent to Broker", "Reject" },
                ["Sent to Broker"] = new List<string> { "Filled", "Sent to Broker", "Sent to Broker Update" },
                ["Filled"] = new List<string> { "Send to Custodian", "Delivered", "Settled" },
                ["Send to Custodian"] = new List<string> { "Delivered", "Settled" },
                ["Delivered"] = new List<string> { "Settled" },
                ["Settled"] = new List<string> { "Send to Custodian", "Delivered" },
                ["Completed"] = new List<string>(),
                ["Rejected"] = new List<string>()
            };

            if (validTransitions.TryGetValue(currentStatus, out var allowedStatuses))
            {
                return allowedStatuses;
            }

            return new List<string>();
        }
    }
}