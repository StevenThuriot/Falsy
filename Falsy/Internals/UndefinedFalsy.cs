#region License

//  
// Copyright 2015 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 

#endregion

using System.Collections;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;
using Horizon;

namespace Falsy.NET.Internals
{
    [DebuggerDisplay("undefined", Name = "Falsy")]
    public sealed class UndefinedFalsy : DynamicFalsy, IEnumerable
    {
        internal static readonly dynamic Value = new UndefinedFalsy();

        //Allow only one instance.
        private UndefinedFalsy()
        {
        }

        public override string ToString()
        {
            return "undefined";
        }

	    public IEnumerator GetEnumerator()
	    {
		    yield break;
	    }

	    public override bool IsFalsyEquivalent()
        {
            return false;
        }

        public override bool IsFalsyNull()
        {
            return true;
        }

        public override bool IsFalsyNaN()
        {
            return false;
        }

        protected internal override bool GetBooleanValue()
        {
            return false;
        }

	    protected internal override dynamic GetValue()
	    {
		    return null;
	    }


	    public override bool Equals(DynamicFalsy arg)
        {
            return Reference.IsNull(arg) || arg.IsFalsyNull();
        }

        protected override bool InternalEquals(object o)
        {
            return Reference.IsNull(o);
        }


        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            if (binder.ReturnType == Constants.BooleanType)
            {
                result = false;
                return true;
            }

            return base.TryConvert(binder, out result);
        }

        public override bool TryBinaryOperation(BinaryOperationBinder binder, object arg, out object result)
        {
            switch (binder.Operation)
            {
                case ExpressionType.Not:
                    result = true;
                    return true;


                case ExpressionType.IsTrue:
                    result = false;
                    return true;

                case ExpressionType.IsFalse:
                    result = true;
                    return true;


                case ExpressionType.Equal:
                    result = Equals(arg);
                    return true;

                case ExpressionType.NotEqual:
                    result = !Equals(arg);
                    return true;


                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    result = false;
                    return true;


                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    if (binder.ReturnType == Constants.BooleanType)
                    {
                        result = !Equals(arg);
                    }
                    else
                    {
                        if (!Equals(arg))
                        {
                            result = arg;
                        }
                        else
                        {
                            result = false;
                        }
                    }

                    return true;
            }

            return base.TryBinaryOperation(binder, arg, out result);
        }

        public override bool TryUnaryOperation(UnaryOperationBinder binder, out object result)
        {
            if (binder.Operation == ExpressionType.Not)
            {
                result = true;
                return true;
            }

            if (binder.ReturnType == Constants.BooleanType)
            {
                if (binder.Operation == ExpressionType.IsTrue)
                {
                    result = false;
                    return true;
                }

                if (binder.Operation == ExpressionType.IsFalse)
                {
                    result = true;
                    return true;
                }
            }

            return base.TryUnaryOperation(binder, out result);
        }
    }
}