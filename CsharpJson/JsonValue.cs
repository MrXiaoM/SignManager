//
//  JsonValue.cs
//
//  Author:
//       <springrain1991@hotmail.com>
//
//  Copyright (c) 2017 tianxiaoning
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;

namespace CsharpJson
{
    /// <summary>
    /// JsonValue type.
    /// JsonValue的数据类型
    /// </summary>
     public enum JsonType
    {
        NULL = 0x0,
        BOOL = 0x1,
        NUMBER = 0x2,
        STRING = 0x3,
        ARRAY = 0x4,
        OBJECT = 0x5,
        UNDEFINED = 0x80
    }
    /// <summary>
    /// Json value.
    /// JsonValue类型
    /// </summary>
    public sealed class JsonValue
    {
        private BaseType value;
        private JsonType type;
        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonValue"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonValue"/>类的实例，JsonValue类型为JsonType.NULL
        /// </summary>
        public JsonValue()
        {
            type = JsonType.NULL;
            this.value = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonValue"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonValue"/>类的实例，JsonValue类型为JsonType.BOOL
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        public JsonValue(bool value)
        {
            type = JsonType.BOOL;
            this.value = new JsonBool(value);
        }
        public static implicit operator JsonValue(bool value)
        {
            return new JsonValue(value);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonValue"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonValue"/>类的实例，JsonValue类型为JsonType.NUMBER
        /// </summary>
        /// <param name="value">Value.</param>
        public JsonValue(int value)
        {
            type = JsonType.NUMBER;
            this.value = new JsonDouble(value);
        }
        public static implicit operator JsonValue(int value)
        {
            return new JsonValue(value);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonValue"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonValue"/>类的实例，JsonValue类型为JsonType.NUMBER
        /// </summary>
        /// <param name="value">Value.</param>
        public JsonValue(long value)
        {
            type = JsonType.NUMBER;
            this.value = new JsonDouble(value);
        }
        public static implicit operator JsonValue(long value)
        {
            return new JsonValue(value);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonValue"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonValue"/>类的实例，JsonValue类型为JsonType.NUMBER
        /// </summary>
        /// <param name="value">Value.</param>
        public JsonValue(double value)
        {
            type = JsonType.NUMBER;
            this.value = new JsonDouble(value);
        }
        public static implicit operator JsonValue(double value)
        {
            return new JsonValue(value);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonValue"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonValue"/>类的实例，JsonValue类型为JsonType.STRING
        /// </summary>
        /// <param name="value">Value.</param>
        public JsonValue(string value)
        {
            type = JsonType.STRING;
            this.value = new JsonString(value);
        }
        public static implicit operator JsonValue(string value)
        {
            return new JsonValue(value);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonValue"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonValue"/>类的实例，JsonValue类型为JsonType.ARRAY
        /// </summary>
        /// <param name="value">Value.</param>
        public JsonValue(JsonArray value)
        {
            type = JsonType.ARRAY;
            this.value =value;
        }
        public static implicit operator JsonValue (JsonArray value)
        {
            return new JsonValue(value);
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonValue"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonValue"/>类的实例，JsonValue类型为JsonType.OBJECT
        /// </summary>
        /// <param name="value">Value.</param>
        public JsonValue(JsonObject value)
        {
            type = JsonType.OBJECT;
            this.value=value;
        }
        public static implicit operator JsonValue (JsonObject value)
        {
            return new JsonValue(value);
        }
        /// <summary>
        /// Is null.
        /// 当前JsonValue数据是否是JsonType.NULL类型
        /// </summary>
        /// <returns><c>true</c>, if null was ised, <c>false</c> otherwise.</returns>
        public bool IsNull()
        {
            return type == JsonType.NULL;
        }
        /// <summary>
        /// Is bool.
        /// 当前JsonValue数据是否是JsonType.BOOL类型
        /// </summary>
        /// <returns><c>true</c>, if bool was ised, <c>false</c> otherwise.</returns>
        public bool IsBool()
        {
            return type == JsonType.BOOL;
        }
        /// <summary>
        /// Is double.
        /// 当前JsonValue数据是否是JsonType.NUMBER类型
        /// </summary>
        /// <returns><c>true</c>, if double was ised, <c>false</c> otherwise.</returns>
        public bool IsNumber()
        {
            return type == JsonType.NUMBER;
        }
        /// <summary>
        /// Is string.
        /// 当前JsonValue数据是否是JsonType.STRING类型
        /// </summary>
        /// <returns><c>true</c>, if string was ised, <c>false</c> otherwise.</returns>
        public bool IsString()
        {
            return type == JsonType.STRING;
        }
        /// <summary>
        /// Ises array.
        /// 当前JsonValue数据是否是JsonType.ARRAY类型
        /// </summary>
        /// <returns><c>true</c>, if array was ised, <c>false</c> otherwise.</returns>
        public bool IsArray()
        {
            return type == JsonType.ARRAY;
        }
        /// <summary>
        /// Is object.
        /// 当前JsonValue数据是否是JsonType.OBJECT类型
        /// </summary>
        /// <returns><c>true</c>, if object was ised, <c>false</c> otherwise.</returns>
        public bool IsObject()
        {
            return type == JsonType.OBJECT;
        }
        /// <summary>
        /// Is undefined.
        /// 当前JsonValue数据是否是JsonType.UNDEFINED类型
        /// </summary>
        /// <returns><c>true</c>, if undefined was ised, <c>false</c> otherwise.</returns>
        public bool IsUndefined()
        {
            return type == JsonType.UNDEFINED;
        }
        /// <summary>
        /// Gets the Value Type.
        /// 获取当前JsonValue的值的类型
        /// </summary>
        /// <value>The gettype.</value>
        public JsonType Valuetype
        {
            get { return this.type;}
        }
        /// <summary>
        /// To the bool.
        /// 将JsonValue转为Bool型，如果JsonValue原本不是 JsonType.BOOL型，则引发Format异常
        /// </summary>
        /// <returns><c>true</c>, if bool was toed, <c>false</c> otherwise.</returns>
        public bool ToBool()
        {
            if(!this.IsBool())
            {
                throw new FormatException();
            }
            return ((JsonBool)this.value).Value;
        }
        /// <summary>
        /// To the int.
        /// 将JsonValue转为int型，如果JsonValue原本不是 JsonType.NUMBER型，则引发Format异常
        /// </summary>
        /// <returns>The int.</returns>
        public int ToInt()
        {
            if (!this.IsNumber())
            {
                throw new FormatException();
            }
            return (int)((JsonDouble)this.value).Value;
        }
        /// <summary>
        /// To the long.
        /// 将JsonValue转为long型，如果JsonValue原本不是 JsonType.NUMBER型，则引发Format异常
        /// </summary>
        /// <returns>The long.</returns>
        public long ToLong()
        {
            if (!this.IsNumber())
            {
                throw new FormatException();
            }
            return (long)((JsonDouble)this.value).Value;
        }
        /// <summary>
        /// To the double.
        /// 将JsonValue转为double型，如果JsonValue原本不是 JsonType.NUMBER型，则引发Format异常
        /// </summary>
        /// <returns>The double.</returns>
        public double ToDouble()
        {
            if (!this.IsNumber())
            {
                throw new FormatException();
            }
            return ((JsonDouble)this.value).Value;
        }

        /// <summary>
        /// To the array.
        /// 将JsonValue转为JsonArray型，如果JsonValue原本不是 JsonType.ARRAY型，则引发Format异常
        /// </summary>
        /// <returns>The array.</returns>
        public JsonArray ToArray()
        {
            if (!this.IsArray())
            {
                throw new FormatException();
            }
            return (JsonArray)this.value;
        }
        /// <summary>
        /// To the object.
        /// 将JsonValue转为JsonObject型，如果JsonValue原本不是 JsonType.OBJECT型，则引发Format异常
        /// </summary>
        /// <returns>The object.</returns>
        public JsonObject ToObject()
        {
            if (!this.IsObject())
            {
                throw new FormatException();
            }
            return (JsonObject)this.value;
        }
        /// <summary>
        /// To the string.
        /// 将JsonValue转为string型，如果JsonValue原本不是 JsonType.STRING型，则引发Format异常
        /// </summary>
        /// <returns>The string.</returns>
        public override  string ToString()
        {
            if (!this.IsString())
            {
                throw new FormatException();
            }
            return ((JsonString)this.value).Value;
        }
    }
    /// <summary>
    /// Base type.
    /// 基类，JsonBool，JsonDouble，JsonString，JsonArray，JsonObject都寸Basetype继承
    /// </summary>
     public class BaseType
    {
        public BaseType()
        {
            //nothing
        }
    }
    /// <summary>
    /// Json bool.
    /// JsonBool类型
    /// </summary>
     sealed class JsonBool:BaseType
    {
        /// <summary>
        /// The value.
        /// bool型数据
        /// </summary>
        private bool value;
        public JsonBool(bool val)
        {
            this.value = val;
        }
        public bool Value
        {
            get{ return this.value;}
            set{this.value=value;}
        }

    }
    /// <summary>
    /// Json double.
    /// JsonDouble类型
    /// </summary>
    sealed class JsonDouble :BaseType
    {
        private double value;
        public JsonDouble(double val)
        {
            this.value =val;
        }
        public double Value
        {
            get{ return this.value;}
            set{this.value=value;}
        }
    }
    /// <summary>
    /// Json string.
    /// </summary>
    sealed class JsonString :BaseType
    {
        private string value;
        public JsonString(string val)
        {
            this.value=val;
        }
        public string Value
        {
            get{ return this.value;}
            set{this.value=value;}
        }

    }
}

