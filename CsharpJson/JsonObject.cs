//
//  JsonObject.cs
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
using System.Collections;
using System.Collections.Generic;

namespace CsharpJson
{
    /// <summary>
    /// Json object.
    /// Json的Objcet型数据，其中的元素都是Key/Value对
    /// </summary>
    public sealed class JsonObject : BaseType, IEnumerable
    {
        /// <summary>
        /// The items.
        /// 保存键值型数据
        /// </summary>
        Dictionary<string, JsonValue> items;
        /// <summary>
        /// Initializes a new instance of the <see cref="CsharpJson.JsonObject"/> class.
        /// 初始化一个新的<see cref="CsharpJson.JsonObject"/>类实例
        /// </summary>
        public JsonObject()
        {
            this.items = new Dictionary<string, JsonValue>();
        }
        /// <summary>
        /// Gets or sets the <see cref="CsharpJson.JsonObject"/> with the string key.
		/// 获取或设置指定Key的<see cref="CsharpJson.JsonObject"/> ;
		/// 获取指定key的值，如果不包含该元素则返回null
        /// </summary>
        /// <param name="key">Key.</param>
        public JsonValue this[string key]
        {
            get
            {
                if (this.items.ContainsKey(key))
                {
                    return items[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (items.ContainsKey(key))
                {
                    this.items[key] = value;
                }
                else
                {
                    if (value==null)
                    {
                        this.items.Add(key,new JsonValue());
                    }
                    else
                    { 
                    this.items.Add(key, value);
                    }
                }
            }
        }
        /// <summary>
        /// Add the specified key and value.
		/// 添加一个指定的键值对
        /// </summary>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        /// <exception cref="ArgumentNullException">当key为null时引发异常</exception>
        public void Add(string key, JsonValue value)
        {
            if (key == null)
            {
                throw new ArgumentNullException();
            }
            if (this.items.ContainsKey(key))
            {
                this.items[key] = value;
            }
            else
            {
                if (value == null)
                {
                    this.items.Add(key, new JsonValue());
                }
                else
                {
                    this.items.Add(key, value);
                }
            }
        }
        /// <summary>
        /// Gets the count.
        /// 获取JsonObject中元素（键/值）的数量
        /// </summary>
        /// <value>The count.</value>
        public int Count
        {
            get
            {
                return this.items.Count;
            }
        }
        /// <summary>
        /// Gets the keys.
        /// 获取通过迭代器获取键
        /// </summary>
        /// <value>The keys.</value>
        public ICollection<string> Keys
        {
            get
            {
                return this.items.Keys;
            }
        }
        /// <summary>
        /// Gets the values.
        /// 通过迭代获取值
        /// </summary>
        /// <value>The values.</value>
        public ICollection<JsonValue> Values
        {
            get
            {
                return this.items.Values;
            }
        }
        /// <summary>
        /// Clear this instance.
        /// 清楚JsonObject中的元素
        /// </summary>
        public void Clear()
        {
            this.items.Clear();
        }
        /// <summary>
        /// if Containses the key return true otherwise return false.、
        /// 如果包含指定的Key,则返回true，其他情况返回false
        /// </summary>
        /// <returns><c>true</c>, if key was containsed, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        public bool ContainsKey(string key)
        {
            return this.items.ContainsKey(key);
        }
        /// <summary>
        /// Remove the item by specified key.
        /// 删除指定key处的元素
        /// </summary>
        /// <param name="key">Key.</param>
        public bool Remove(string key)
        {
            if (this.ContainsKey(key))
            {
                this.items.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Tries the get value.
        /// 尝试获取指定key的值，如果key存在返回ture，否则返回false
        /// </summary>
        /// <returns><c>true</c>, if get value was tryed, <c>false</c> otherwise.</returns>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public bool TryGetValue(string key, out JsonValue value)
        {
            if (this.items.ContainsKey(key))
            {
                value = this.items[key];
                return true;
            }
            else
            {
                value = null;
                return false;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }
        /// <summary>
        /// Value the specified key.
        /// 返回指定key的value
        /// </summary>
        /// <param name="key">Key.</param>
        public JsonValue Value(string key)
        {
            return this.items[key];
        }

    }
}
