using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace ProjectEstimationTool.Classes
{
    [Serializable]
    public abstract class ExceptionArgs
    {
        public virtual String Message => String.Empty;
    }

    [Serializable]
    public sealed class Exception<TExceptionArgs> : Exception, ISerializable where TExceptionArgs: ExceptionArgs
    {
        private const String ARGS = "Args";
        private readonly TExceptionArgs mArgs;

        public Exception(String message = null, Exception innerException = null)
            :   this(null, message, innerException)
        {
        }

        public Exception(TExceptionArgs args, String message = null, Exception innerException = null)
            :   base(message, innerException)
        {
            this.mArgs = args;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        private Exception(SerializationInfo info, StreamingContext context)
            :   base(info, context)
        {
            this.mArgs = (TExceptionArgs)info.GetValue(ARGS, typeof(TExceptionArgs));
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(ARGS, this.mArgs);
            base.GetObjectData(info, context);
        }

        public TExceptionArgs Args => this.mArgs;

        public override String Message
        {
            get
            {
                String baseMessage = base.Message;
                return (this.mArgs == null) ? baseMessage : String.Format("{0} (1})", baseMessage, this.mArgs.Message);
            }
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Exception<TExceptionArgs> other = obj as Exception<TExceptionArgs>;
            return Object.Equals(mArgs, other.mArgs) && base.Equals(obj);
        }

        public override Int32 GetHashCode()
        {
            return base.GetHashCode();
        }
    } // class ProjectEstimationToolException
} // namespace ProjectEstimationTool.Classes
