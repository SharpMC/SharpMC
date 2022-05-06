using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using SharpNBT;

namespace SharpMC.Network.Binary
{
    public abstract class NbtPoco
    {
        protected T GetPropertyValue<T>(TagContainer tag, Expression<Func<T>> property)
        {
            var propertyInfo = ((MemberExpression) property.Body).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            var nbtTag = tag.FirstOrDefault(t => t.Name == propertyInfo.Name);
            if (nbtTag == null)
            {
                nbtTag = tag.FirstOrDefault(t => t.Name == LowercaseFirst(propertyInfo.Name));
            }

            var mex = property.Body as MemberExpression;
            if (nbtTag == null || mex?.Expression == null)
            {
                return default;
            }

            var target = Expression.Lambda(mex.Expression).Compile().DynamicInvoke();
            switch (nbtTag.Type)
            {
                case TagType.End:
                    break;
                case TagType.Byte:
                    if (propertyInfo.PropertyType == typeof(bool))
                        propertyInfo.SetValue(target, ((ByteTag) nbtTag).Value == 1);
                    else
                        propertyInfo.SetValue(target, ((ByteTag) nbtTag).Value);
                    break;
                case TagType.Short:
                    propertyInfo.SetValue(target, ((ShortTag) nbtTag).Value);
                    break;
                case TagType.Int:
                    if (propertyInfo.PropertyType == typeof(bool))
                        propertyInfo.SetValue(target, ((IntTag) nbtTag).Value == 1);
                    else
                        propertyInfo.SetValue(target, ((IntTag) nbtTag).Value);
                    break;
                case TagType.Long:
                    propertyInfo.SetValue(target, ((LongTag) nbtTag).Value);
                    break;
                case TagType.Float:
                    propertyInfo.SetValue(target, ((FloatTag) nbtTag).Value);
                    break;
                case TagType.Double:
                    propertyInfo.SetValue(target, ((DoubleTag) nbtTag).Value);
                    break;
                case TagType.ByteArray:
                    propertyInfo.SetValue(target, ((ByteArrayTag) nbtTag).ToArray());
                    break;
                case TagType.String:
                    propertyInfo.SetValue(target, ((StringTag) nbtTag).Value);
                    break;
                case TagType.List:
                    break;
                case TagType.Compound:
                    break;
                case TagType.IntArray:
                    propertyInfo.SetValue(target, ((IntArrayTag) nbtTag).ToArray());
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nbtTag.Type} ?!");
            }

            var raw = propertyInfo.GetValue(target);
            return (T) raw;
        }

        protected T SetPropertyValue<T>(TagContainer tag, Expression<Func<T>> property)
        {
            var propertyInfo = ((MemberExpression) property.Body).Member as PropertyInfo;
            if (propertyInfo == null)
            {
                throw new ArgumentException("The lambda expression 'property' should point to a valid Property");
            }

            var nbtTag = tag.FirstOrDefault(t => t.Name == propertyInfo.Name);
            if (nbtTag == null)
            {
                nbtTag = tag.FirstOrDefault(t => t.Name == LowercaseFirst(propertyInfo.Name));
            }

            var mex = property.Body as MemberExpression;
            if (nbtTag == null || mex?.Expression == null)
            {
                return default;
            }

            var target = Expression.Lambda(mex.Expression).Compile().DynamicInvoke();
            switch (nbtTag.Type)
            {
                case TagType.End:
                    break;
                case TagType.Byte:
                    if (propertyInfo.PropertyType == typeof(bool))
                        tag.Add(new ByteTag(nbtTag.Name, (byte) ((bool) propertyInfo.GetValue(target)! ? 1 : 0)));
                    else
                        tag.Add(new ByteTag(nbtTag.Name, (byte) propertyInfo.GetValue(target)!));
                    break;
                case TagType.Short:
                    tag.Add(new ShortTag(nbtTag.Name, (short) propertyInfo.GetValue(target)!));
                    break;
                case TagType.Int:
                    if (propertyInfo.PropertyType == typeof(bool))
                        tag.Add(new IntTag(nbtTag.Name, (bool) propertyInfo.GetValue(target)! ? 1 : 0));
                    else
                        tag.Add(new IntTag(nbtTag.Name, (int) propertyInfo.GetValue(target)!));
                    break;
                case TagType.Long:
                    tag.Add(new LongTag(nbtTag.Name, (long) propertyInfo.GetValue(target)!));
                    break;
                case TagType.Float:
                    tag.Add(new FloatTag(nbtTag.Name, (float) propertyInfo.GetValue(target)!));
                    break;
                case TagType.Double:
                    tag.Add(new DoubleTag(nbtTag.Name, (double) propertyInfo.GetValue(target)!));
                    break;
                case TagType.ByteArray:
                    tag.Add(new ByteArrayTag(nbtTag.Name, (byte[]) propertyInfo.GetValue(target)!));
                    break;
                case TagType.String:
                    tag.Add(new StringTag(nbtTag.Name, (string) propertyInfo.GetValue(target)!));
                    break;
                case TagType.List:
                    break;
                case TagType.Compound:
                    break;
                case TagType.IntArray:
                    tag.Add(new IntArrayTag(nbtTag.Name, (int[]) propertyInfo.GetValue(target)!));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"{nbtTag.Type} ?!");
            }

            var raw = propertyInfo.GetValue(target);
            return (T) raw;
        }

        private static string LowercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToLower(s[0]) + s.Substring(1);
        }
    }
}