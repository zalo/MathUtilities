//Copyright (c) 2014 Ivan Gusiev <ivan.gusiev@gmail.com>
//https://github.com/ivan-gusiev/autodiff/
namespace AutoDiff
{
        /*	represents a number in form n = <r, d>
        where r-part is real number, and d-part is its derivative
       
        one has 3 options to create DualNumber:
            * use static function DualNumber.Variable(x)
              and get <x, 1>
            * use static function DualNumber.Constant(x)
              and get <x, 0>
            * create structure with default value
              and get <0, 0>
        
        this structure is immutable
    */
    public struct DualNumber
    {
        public float Value
        {
            get;
            private set;
        }

        public float Derivative
        {
            get;
            private set;
        }

        #region Construction
        
        /// <summary>
        /// Only member methods and methods from Math module can create 
        /// other dual numbers by directly specifying real and derivative parts
        /// </summary>
        /// <param name="value">Value parameter</param>
        /// <param name="derivative">Derivative parameter</param>
        internal DualNumber(float value, float derivative) : this()
        {
            Value = value;
            Derivative = derivative;
        }

        /// <summary>
        /// Creates a dual variable, d(x) == 1
        /// </summary>
        /// <param name="value">Real value of variable</param>
        public static DualNumber Variable(float value) 
        {
            return new DualNumber(value, 1);
        }

        /// <summary>
        /// Creates a dual constant, d(c) == 0
        /// </summary>
        /// <param name="value">Real value of constant</param>
        public static DualNumber Constant(float value)
        {
            return new DualNumber(value, 0);
        }

        #endregion

        #region Operators 

        public static implicit operator DualNumber(float value)
        {
            return Constant(value);
        }

        public static implicit operator float(DualNumber value) 
        {
            return value.Value;
        }

        public static DualNumber operator+ (DualNumber first, DualNumber second)
        {
            // d(a + b) == d(a) + d(b)
            return new DualNumber( first.Value + second.Value, 
                                   first.Derivative + second.Derivative );
        }
        
        public static DualNumber operator- (DualNumber number)
        {
            // d(-x) == -d(x)
            return new DualNumber( -number.Value, -number.Derivative );
        }
            
        public static DualNumber operator- (DualNumber first, DualNumber second)
        {
            // d(a - b) == d(a) + d(-b)
            return first + (-second);
        }
        
        public static DualNumber operator* (DualNumber first, DualNumber second)
        {
            // d(a * b) == d(a)*b + a*d(b)
            return new DualNumber (first.Value * second.Value,
                                   first.Derivative * second.Value + second.Derivative * first.Value);
        }
        
        public static DualNumber operator/ (DualNumber first, DualNumber second)
        {
            // d(a * b) == ( d(a)*b - a*d(b) ) / b^2
            var derivNumerator = first.Derivative * second.Value - second.Derivative * first.Value;
            return new DualNumber( first.Value / second.Value,
                                   derivNumerator / (second.Value * second.Value));
        }

        #endregion

        public override string ToString()
        {
            return string.Format("<{0}; {1}>", Value, Derivative);
        }
    }
}
