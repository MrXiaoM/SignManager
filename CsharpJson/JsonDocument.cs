//
//  JsonDocument.cs
//
//  Author:
//       tianxiaoning <springrain1991@hotmail.com>
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
using System.Collections.Generic;
using System.Text;

namespace CsharpJson
{
    /// <summary>
    /// Json document.
    /// 对Json字符串进行反序列化
    /// 对JsonObject和JsonArray进行序列化
    /// </summary>
    public class JsonDocument
    {
        public enum JsonEncoding
        {
            Default,
            ASCII,
            Unicode,
            UTF7,
            UTF8,
            UTF32,
            BigEndianUnicode
        }

        //默认字符串的平均最大长度
        //为了提高字符串的拼接性能，需要提前预估分配内存，避免StringBuild频繁申请内存
        private readonly int DEFAULT_MAX_LENGHT = 40;
        private JsonObject _object = null;
        private JsonArray array = null;
        private JsonEncoding encoding = JsonEncoding.Default;
        /// <summary>
        /// Gets or sets the encod.
        /// 获取或设置编码格式
        /// </summary>
        /// <value>The encod.</value>
        public JsonEncoding Encod 
        {
            get { return encoding; }
            set { encoding = value; }
        }
        /// <summary>
        /// Gets or sets the object.
        /// 获取或设置解析/被解析的JsonObject对象
        /// </summary>
        /// <value>The object.</value>
        public JsonObject Object
        {
            get { return _object; }
            set {
                _object = value;
                array = null; }

        }
        /// <summary>
        /// Gets or sets the array.
        /// 获取或设置解析/被解析的JsonArray数组
        /// </summary>
        /// <value>The array.</value>
        public JsonArray Array
        {
            get { return array; }
            set { array = value; _object = null; }
        }
        /// <summary>
        /// Determines whether this instance is object.
        /// 判断当前JsonDocument包含的是否是JsonObject,是返回true，其他情况返回false
        /// </summary>
        /// <returns><c>true</c> if this instance is object; otherwise, <c>false</c>.</returns>
        public bool IsObject()
        {
            if (Object != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Determines whether this instance is array.
        /// 判断当前JsonDocument包含的是否是JsonArray，是返回true，其他情况返回false
        /// </summary>
        /// <returns><c>true</c> if this instance is array; otherwise, <c>false</c>.</returns>
        public bool IsArray()
        {
            if (Array != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Determines whether this instance is null.
        /// 判断当前JsonDocument是否为空，是返回true，其他情况返回false
        /// </summary>
        /// <returns><c>true</c> if this instance is null; otherwise, <c>false</c>.</returns>
        public bool IsNull()
        {
            if (Object == null && Array == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Determines whether this instance is empty.
        /// 判断当前JsonDocument包含的JsonObject或JsonArray是否不包含任何元素;
        /// 当即不包含JsonObject又不包含JsonArray时返回true;
        /// 当JsonObject或JsonArray元素数为0时返回true，其他情况返回false
        /// </summary>
        /// <returns><c>true</c> if this instance is empty; otherwise, <c>false</c>.</returns>
        public bool IsEmpty()
        {
            if (Object == null && Array == null)
            {
                return true;
            }
            if (Object != null && Object.Count == 0)
            {
                return true;
            }
            else if (Array != null && Array.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        } 
        /// <summary>
        /// Tos the json array.
        /// 序列化成Byte数组
        /// </summary>
        /// <returns>The json array.</returns>
        public byte[] ToJsonArray()
        {
            return GetBytes(ToJson());
        }
        /// <summary>
        /// 序列化为字符串
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NullReferenceException">序列化对象为空引用</exception>
        public string ToJson()
        {
            if (Object != null)
            {
                string jsonstr = ToJsonString(Object);
                return jsonstr;
            }
            else if (Array != null)
            {
                string jsonstr = ToJsonString(Array);
                return jsonstr;
            }
            else
            {
                throw new NullReferenceException("被序列化的对象或数组不能为null");
            }
        }
        /// <summary>
        /// Froms the json.
        /// 从byte数组获取JsonDocument
        /// </summary>
        /// <returns>The json.返回JsonDocument对象</returns>
        /// <param name="json">Json.Json数组</param>
        /// <param name="encod">Encod.编码，默认使用当前默认编码</param>
        public static JsonDocument FromJson(byte[] json, JsonEncoding encod = JsonEncoding.Default)
        {
            switch (encod)
            {
                case JsonEncoding.ASCII:
                    return FromString(Encoding.ASCII.GetString(json));
                case JsonEncoding.Unicode:
                    return FromString(Encoding.Unicode.GetString(json));
                case JsonEncoding.UTF7:
                    return FromString(Encoding.UTF7.GetString(json));
                case JsonEncoding.UTF8:
                    return FromString(Encoding.UTF8.GetString(json));
                case JsonEncoding.UTF32:
                    return FromString(Encoding.UTF32.GetString(json));
                case JsonEncoding.BigEndianUnicode:
                    return FromString(Encoding.BigEndianUnicode.GetString(json));
                case JsonEncoding.Default:
                default:
                    return FromString(Encoding.Default.GetString(json));
            }
        }
        /// <summary>
        /// Froms the string.
        /// 从Json字符串获取JsonDocument
        /// </summary>
        /// <returns>The Json.返回JsonDocument对象</returns>
        /// <param name="jsonstr">Jsonstr.Json字符串</param>
        /// <param name="encod">Encod.编码，默认使用当前默认编码</param>
        /// <exception cref="NullReferenceException">Json字符串为null引用异常</exception>
        public static JsonDocument FromString(string jsonstr, JsonEncoding encod = JsonEncoding.Default)
        {
            JsonDocument doc = new JsonDocument();
            doc.Encod = encod;
            if (jsonstr == null)
            {
                throw new NullReferenceException("不能为null");
            }
            int index = 0;
            if (jsonstr[0] == '{')
            {
                doc.Object = GetObject(ref index, jsonstr).ToObject();
            }
            else if (jsonstr[0] == '[')
            {
                doc.Array = GetArray(ref index, jsonstr).ToArray();
            }
            return doc;
        }

        /// <summary>
        /// Gets the bytes.
        /// 将序列化的Json字符串转成byte
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="jsonstr">Jsonstr.</param>
        private byte[] GetBytes(string jsonstr)
        {
            if (jsonstr == null)
            {
                return null;
            }
            switch (Encod)
            {
                case JsonEncoding.ASCII:
                    return Encoding.ASCII.GetBytes(jsonstr);
                case JsonEncoding.Unicode:
                    return Encoding.Unicode.GetBytes(jsonstr);
                case JsonEncoding.UTF7:
                    return Encoding.UTF7.GetBytes(jsonstr);
                case JsonEncoding.UTF8:
                    return Encoding.UTF8.GetBytes(jsonstr);
                case JsonEncoding.UTF32:
                    return Encoding.UTF32.GetBytes(jsonstr);
                case JsonEncoding.BigEndianUnicode:
                    return Encoding.BigEndianUnicode.GetBytes(jsonstr);
                case JsonEncoding.Default:
                default:
                    return Encoding.Default.GetBytes(jsonstr);
            }
        }
        /// <summary>
        /// To the json string.
        /// 将JsonObject序列化成字符串
        /// </summary>
        /// <returns>The json string.</returns>
        private string ToJsonString(JsonObject items, int intentStep = 4, int intent = 0)
        {
            var intent1 = new string(' ', intent);
            var intent2 = new string(' ', intent + intentStep);
            StringBuilder str = new StringBuilder(items.Count * 2 * DEFAULT_MAX_LENGHT);
            str.Append("{");
            if(items.Count==0)
            {
                str.Append("}");
                return str.ToString();
            }
            else
            {
                str.Append("\n");
            }
            foreach (KeyValuePair<string, JsonValue> item in items)
            {
                switch (item.Value.Valuetype)
                {
                    case JsonType.NULL:
                        str.AppendFormat(intent2 + "\"{0}\": {1},\n", item.Key, "null");
                        break;
                    case JsonType.BOOL:
                        str.AppendFormat(intent2 + "\"{0}\": {1},\n", item.Key, item.Value.ToBool().ToString().ToLower());
                        break;
                    case JsonType.NUMBER:
                        str.AppendFormat(intent2 + "\"{0}\": {1},\n", item.Key, item.Value.ToDouble());
                        break;
                    case JsonType.STRING:
                        str.AppendFormat(intent2 + "\"{0}\": \"{1}\",\n", item.Key, item.Value.ToString().Replace("\\", "\\\\").Replace("\"","\\\""));
                        break;
                    case JsonType.ARRAY:
                        str.AppendFormat(intent2 + "\"{0}\": {1},\n", item.Key, ToJsonString(item.Value.ToArray(), intentStep, intent + intentStep));
                        break;
                    case JsonType.OBJECT:
                        str.AppendFormat(intent2 + "\"{0}\": {1},\n", item.Key, ToJsonString(item.Value.ToObject(), intentStep, intent + intentStep));
                        break;
                }
            }
            str.Replace(",\n", "\n" + intent1 + "}", str.Length - 2, 2);
            return str.ToString();
        }
        /// <summary>
        /// Tos the json string.
        /// 将JsonArray序列化成Json字符串
        /// </summary>
        /// <returns>The json string.</returns>
        /// <param name="arrylist">Arrylist.</param>
        private string ToJsonString(JsonArray arrylist, int intentStep = 4, int intent = 0)
        {
            var intent1 = new string(' ', intent);
            var intent2 = new string(' ', intent + intentStep);
            StringBuilder str = new StringBuilder(arrylist.Count * DEFAULT_MAX_LENGHT);
            str.Append("[");
            if(arrylist.Count==0)
            {
                str.Append("]");
                return str.ToString();
            }
            else
            {
                str.Append("\n");
            }
            foreach (JsonValue item in arrylist)
            {
                switch (item.Valuetype)
                {
                    case JsonType.NULL:
                        str.AppendFormat(intent2 + "{0},\n", "null");
                        break;
                    case JsonType.BOOL:
                        str.AppendFormat(intent2 + "{0},\n", item.ToBool().ToString().ToLower());
                        break;
                    case JsonType.NUMBER:
                        str.AppendFormat(intent2 + "{0},\n", item.ToDouble());
                        break;
                    case JsonType.STRING:
                        str.AppendFormat(intent2 + "\"{0}\",\n", item.ToString().Replace("\\","\\\\").Replace("\"", "\\\""));
                        break;
                    case JsonType.ARRAY:
                        str.AppendFormat(intent2 + "{0},\n", ToJsonString(item.ToArray(), intentStep, intent + intentStep));
                        break;
                    case JsonType.OBJECT:
                        str.AppendFormat(intent2 + "{0},\n", ToJsonString(item.ToObject(), intentStep, intent + intentStep));
                        break;
                }
            }
            str.Replace(",\n", "\n" + intent1 + "]", str.Length - 2, 2);
            return str.ToString();
        }
        /// <summary>
        /// Gets the object.
        /// 获取JsonObject通过Json字符串，即解析Object型字符串
        /// </summary>
        /// <returns>The object.</returns>
        /// <param name="index">Index.</param>
        /// <param name="strjson">Strjson.</param>
        /// <exception cref="FormatException">Json格式错误异常</exception>
        static private JsonValue GetObject(ref int index, string strjson)
        {
            index++;
            JsonObject obj = new JsonObject();
            string key = "";
            bool iskey = true;
            while (index < strjson.Length)
            {
                switch (strjson[index])
                {
                    case ':':
                        iskey = false;
                        index++;
                        break;
                    case ' ':
                    case '\n':
                    case '\r':
                        index++;
                        break;
                    case '"':
                        if (iskey)
                        {
                            key = GetString(ref index, strjson);
                        }
                        else
                        {
                            obj.Add(key, GetString(ref index, strjson));
                        }
                        break;
                    case '{':
                        obj.Add(key, GetObject(ref index, strjson));
                        break;
                    case 't':
                    case 'f':
                        obj.Add(key, GetBool(ref index, strjson));
                        break;
                    case 'n':
                        obj.Add(key,GetNull(ref index,strjson));
                        break;
                    case ',':
                        iskey = true;
                        key = "";
                        index++;
                        break;
                    case '[':
                        obj.Add(key, GetArray(ref index, strjson));
                        break;
                    case '}':
                        ++index;
                        return obj;
                    default:
                        obj.Add(key, GetNumber(ref index, strjson));
                        break;
                }
            }
            throw new FormatException("解析错误,似乎不是一个完整Json！");
        }
        /// <summary>
        /// Gets the array.
        /// 获取JsonArray通过Json字符串，即解析Array型字符串
        /// </summary>
        /// <returns>The array.</returns>
        /// <param name="index">Index.</param>
        /// <param name="strjson">Strjson.</param>
        static private JsonValue GetArray(ref int index, string strjson)
        {
            index++;
            JsonArray arr = new JsonArray();
            while (index < strjson.Length)
            {
                switch (strjson[index])
                {
                    case '"':
                        arr.Add(GetString(ref index, strjson));
                        break;
                    case ' ':
                    case '\n':
                    case '\r':
                        ++index;
                        break;
                    case '{':
                        arr.Add(GetObject(ref index, strjson));
                        break;
                    case 't':
                    case 'f':
                        arr.Add(GetBool(ref index, strjson));
                        break;
					case 'n':
						arr.Add(GetNull(ref index,strjson));
					break;
                    case ',':
                        index++;
                        break;
                    case '[':
                        arr.Add(GetArray(ref index, strjson));
                        break;
                    case ']':
                        ++index;
                        return arr;
                    default:
                        arr.Add(GetNumber(ref index, strjson));
                        break;
                }
            }
            return null;
        }
        /// <summary>
        /// Gets the string.
        /// 获取string型数据
        /// </summary>
        /// <returns>The string.</returns>
        /// <param name="index">Index.</param>
        /// <param name="jsonstr">Jsonstr.</param>
        /// <exception cref="FormatException">Json格式错误异常</exception>
        static private string GetString(ref int index, string jsonstr)
        {
            string str = "";
            index++;
            for (; index < jsonstr.Length; ++index)
            {
                switch (jsonstr[index])
                {
                    case '"':
                        index++;
                        return str;
                    case '\\':
                        if(index+1>=jsonstr.Length)
                        {
                            break;
                        }
                        if(jsonstr[index+1]=='"'|| jsonstr[index + 1] == '\\')
                        {
                            index++;
                        }
                        str += jsonstr[index];
                        break;
                    default:
                        str += jsonstr[index];
                        break;
                }
            }
            throw new FormatException("在\"" + str + "\"位置Json格式不正确！");
        }
        /// <summary>
        /// Getbool the specified index and strjson.
        /// 获取Bool型数据
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="strjson">Strjson.</param>
        /// <exception cref="FormatException">Json格式错误异常</exception>
        static private JsonValue GetBool(ref int index, string strjson)
        {
            string strbool = "";
            for (; index < strjson.Length; ++index)
            {
                if (strjson[index] == ',' ||strjson[index] == '}' || strjson[index] == ']'||strjson[index] == ' '||strjson[index] == '\n'|| strjson[index] == '\r')
                {
                    break;
                }
                else
                {
                    strbool += strjson[index];
                }
            }
            if (strbool == "true")
            {
                return new JsonValue(true);
            }
            else if (strbool == "false")
            {
                return new JsonValue(false);
            }
            else
            {
                throw new FormatException("在\"" + strbool + "\"位置Json格式不正确！");
            }
        }
        /// <summary>
        /// Getnumber the specified index and strjson.
        /// 获取数字型数据
        /// </summary>
        /// <param name="index">Index.</param>
        /// <param name="strjson">Strjson.</param>
        /// <exception cref="FormatException">Json格式错误异常</exception>
        static private JsonValue GetNumber(ref int index, string strjson)
        {
            string strnum = "";
            for (; index < strjson.Length; ++index)
            {
                 if (strjson[index] == ',' || strjson[index] == '}'|| strjson[index] == ']'||strjson[index] == ' ' || strjson[index] == '\n' || strjson[index] == '\r')
                {
                    break;
                }
                else
                {
                    strnum += strjson[index];
                }
            }
            double value;
            bool b = double.TryParse(strnum, out value);
            if (b)
            {
                return value;
            }
            else
            {
                throw new FormatException("在\"" + strnum + "\"位置Json格式不正确！");
            }
        }
        /// <summary>
        /// Gets the null.
        /// 获取null型数据
        /// </summary>
        /// <returns>The null.</returns>
        /// <param name="index">Index.</param>
        /// <param name="strjson">Strjson.</param>
        /// <exception cref="FormatException">Json格式错误异常</exception>
        static private JsonValue GetNull(ref int index,string strjson)
        {
            string strnull = "";
            for (; index < strjson.Length; ++index)
            {
                if (strjson[index] == ',' || strjson[index] == '}' || strjson[index] == ']'||strjson[index] == ' ' || strjson[index] == '\n' ||strjson[index] == '\r')
                {
                    break;
                }
                else
                {
                    strnull += strjson[index];
                }
            }
            if (strnull == "null")
            {
                return new JsonValue();
            }
            else
            {
                throw new FormatException("在\""+strnull+"\"位置Json格式不正确！");
            }

        }

    }
}
