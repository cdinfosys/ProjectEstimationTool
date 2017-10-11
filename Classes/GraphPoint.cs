using System;

namespace ProjectEstimationTool.Classes
{
    public struct GraphPoint : IEquatable<GraphPoint>
    {
        private Int32 mX;
        private Int32 mY;

        public GraphPoint(Int32 x, Int32 y)
        {
            this.mX = x;
            this.mY = y;
        }

        public Int32 X => this.mX;
        public Int32 Y => this.mY;

        public override Int32 GetHashCode()
        {
            return mX ^ mY;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != this.GetType()) return false;

            GraphPoint gp = (GraphPoint)obj;
            if ((this.mX != gp.mX) || (this.mY != gp.mY)) return false;

            return true;
        }

        public override String ToString()
        {
            return String.Format("{0},{1}", mX.ToString(), mY.ToString());
        }

        public Boolean Equals(GraphPoint other)
        {
            if ((this.mX != other.mX) || (this.mY != other.mY)) return false;

            return true;
        }
    } // class GraphPoint
} // namespace ProjectEstimationTool.Classes
