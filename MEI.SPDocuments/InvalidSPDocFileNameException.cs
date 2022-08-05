using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

using MEI.SPDocuments.TypeCodes;

namespace MEI.SPDocuments
{
    /// <summary>
    ///     Represents errors that occur when documents are created with invalid file names.
    /// </summary>
    [Serializable]
    public class InvalidSPDocFileNameException
        : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidSPDocFileNameException" /> class.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        public InvalidSPDocFileNameException(string message)
            : base(message)
        {
            EType = InvalidSPDocFileNameExceptionType.Undefined;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidSPDocFileNameException" /> class.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <param name="exceptionType">Type of the fileName exception.</param>
        public InvalidSPDocFileNameException(string message, InvalidSPDocFileNameExceptionType exceptionType)
            : base(message)
        {
            EType = exceptionType;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidSPDocFileNameException" /> class.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public InvalidSPDocFileNameException(string message, Exception innerException)
            : base(message, innerException)
        {
            EType = InvalidSPDocFileNameExceptionType.Undefined;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidSPDocFileNameException" /> class.
        /// </summary>
        /// <param name="message">The message of the error.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="exceptionType">Type of the fileName exception.</param>
        public InvalidSPDocFileNameException(string message, Exception innerException, InvalidSPDocFileNameExceptionType exceptionType)
            : base(message, innerException)
        {
            EType = exceptionType;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidSPDocFileNameException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        /// <param name="exceptionType">Type of the fileName exception.</param>
        protected InvalidSPDocFileNameException(SerializationInfo info,
                                                StreamingContext context,
                                                InvalidSPDocFileNameExceptionType exceptionType)
            : base(info, context)
        {
            EType = exceptionType;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="InvalidSPDocFileNameException" /> class.
        /// </summary>
        /// <param name="info">The info.</param>
        /// <param name="context">The context.</param>
        protected InvalidSPDocFileNameException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            EType = (InvalidSPDocFileNameExceptionType)info.GetValue("exceptionTypeCode", EType.GetType());
        }

        /// <summary>
        ///     Gets or sets the type of fileName exception.
        /// </summary>
        public InvalidSPDocFileNameExceptionType EType { get; }

        /// <summary>
        ///     When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with
        ///     information about the exception.
        /// </summary>
        /// <param name="info">
        ///     The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object
        ///     data about the exception being thrown.
        /// </param>
        /// <param name="context">
        ///     The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual
        ///     information about the source or destination.
        /// </param>
        /// <exception cref="T:System.ArgumentNullException">
        ///     The <paramref name="info" /> parameter is a null reference (Nothing in Visual Basic).
        /// </exception>
        /// <PermissionSet>
        ///     <IPermission
        ///         class="System.Security.Permissions.FileIOPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Read="*AllFiles*" PathDiscovery="*AllFiles*" />
        ///     <IPermission
        ///         class="System.Security.Permissions.SecurityPermission, mscorlib, Version=2.0.3600.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"
        ///         version="1" Flags="SerializationFormatter" />
        /// </PermissionSet>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("exceptionTypeCode", EType);
        }
    }
}
