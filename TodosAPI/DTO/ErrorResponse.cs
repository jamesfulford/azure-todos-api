using System;

namespace TodosAPI.DTO
{
    /// <summary>
    /// Enumeration of errors to error numbers.
    /// </summary>
    public enum ErrorNumber : long
    {
        /// <summary>
        /// Item already exists.
        /// </summary>
        EXISTS = 1,

        /// <summary>
        /// Parameter is too large/long.
        /// </summary>
        TOOLARGE = 2,

        /// <summary>
        /// Parameter is required.
        /// </summary>
        REQUIRED = 3,

        /// <summary>
        /// Cannot create another item, because it would exceed the configured maximum.
        /// </summary>
        MAXENTITIES = 4,

        /// <summary>
        /// Item with given id is not found.
        /// </summary>
        NOTFOUND = 5,

        /// <summary>
        /// Parameter too small.
        /// </summary>
        TOOSMALL = 6,

        /// <summary>
        /// Parameter is otherwise invalid.
        /// </summary>
        INVALID = 7,
    }

    /// <summary>
    /// Data Transfer Object to serialize various error responses.
    /// </summary>
    public class ErrorResponse
    {
        /// <summary>
        /// Number corresponding to type of error.
        /// </summary>
        /// <remarks>
        /// 1: Entity already exists.
        /// 2: The specified parameter is too large.
        /// 3: The parameter is required.
        /// 4: No more entities can be created at this time.
        /// 5: The entity could not be found.
        /// 6: The parameter is too small.
        /// 7: The parameter is not valid.
        /// </remarks>
        public ErrorNumber errorNumber;

        /// <summary>
        /// Parameter name being referenced in the error, if relevant to error.
        /// </summary>
        public string parameterName;

        /// <summary>
        /// Parameter value violating validation, if relevant to error.
        /// </summary>
        public string parameterValue;

        /// <summary>
        /// A developer-only description of the error.
        /// </summary>
        public string errorDescription;

        /// <summary>
        /// Creates an error.
        /// </summary>
        /// <param name="errorNumber">Error number, indicating type of problem.</param>
        /// <param name="parameterName">Key holding faulty value, if relevant to error.</param>
        /// <param name="parameterValue">Faulty value converted to a string, if relevant to error.</param>
        public ErrorResponse(ErrorNumber errorNumber, string parameterName = null, string parameterValue = null)
        {
            this.errorNumber = errorNumber;
            this.parameterName = parameterName;
            this.parameterValue = parameterValue;
            switch (errorNumber)
            {
                case ErrorNumber.EXISTS:
                    this.errorDescription = "The entity already exists";
                    break;
                case ErrorNumber.TOOLARGE:
                    this.errorDescription = "The parameter value is too large";
                    break;
                case ErrorNumber.REQUIRED:
                    this.errorDescription = "The parameter is required";
                    break;
                case ErrorNumber.MAXENTITIES:
                    this.errorDescription = "The maximum number of entities have been created. No further entities can be created at this time.";
                    break;
                case ErrorNumber.NOTFOUND:
                    this.errorDescription = "The entity could not be found";
                    break;
                case ErrorNumber.TOOSMALL:
                    this.errorDescription = "The parameter value is too small";
                    break;
                case ErrorNumber.INVALID:
                    this.errorDescription = "The parameter value is not valid";
                    break;
                default:
                    this.errorDescription = "An unknown error occurred";
                    break;
            }
        }
    }
}
