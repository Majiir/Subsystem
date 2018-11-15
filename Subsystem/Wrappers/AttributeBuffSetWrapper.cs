using BBI.Core.Data;
using BBI.Core.IO.Streams;
using BBI.Game.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Subsystem.Wrappers
{
    public class AttributeBuffSetWrapper : AttributeBuffSet
    {
        public List<AttributeBuffWrapper> Buffs { get; } = new List<AttributeBuffWrapper>();

        public AttributeBuffSetWrapper()
        {
            UniqueTypeName = nameof(AttributeBuffSetWrapper);
        }

        public AttributeBuffSetWrapper(AttributeBuffSet other)
        {
            UniqueTypeName = other.UniqueTypeName;

            using (var stream = new MemoryStream())
            {
                var writer = new BinaryStreamWriter(stream);
                var reader = new BinaryStreamReader(stream);
                other.Write(writer);
                reader.SeekToStart();
                Read(reader);
            }
        }

        public string UniqueTypeName { get; private set; }

        public IEnumerable<AttributeBuff> GetBuffs(Buff.Category category, string name)
        {
            return Buffs
                .Where(x => x.Category == category)
                .Where(x => string.IsNullOrEmpty(name) || string.IsNullOrEmpty(x.Name) || x.Name == name)
                .Select(x => new AttributeBuff(x.AttributeID, x.Mode, x.Value));
        }

        public void Read(BinaryStreamReader reader)
        {
            var count = reader.ReadInt32();

            Buffs.Clear();

            for (var i = 0; i < count; i++) {

                var name = reader.ReadString();
                var categoryAndId = (Buff.CategoryAndID)reader.ReadInt32();
                var mode = (AttributeBuffMode)reader.ReadInt32();
                var value = reader.ReadInt32();

                Buffs.Add(new AttributeBuffWrapper
                {
                    Name = name,
                    AttributeID = Buff.AttributeIDFromBuffCategoryAndID(categoryAndId),
                    Category = Buff.CategoryFromBuffCategoryAndID(categoryAndId),
                    Mode = mode,
                    Value = value,
                });
            }
        }

        public void Write(BinaryStreamWriter writer)
        {
            writer.WriteInt32(Buffs.Count);

            foreach (var buff in Buffs)
            {
                writer.WriteString(buff.Name);
                writer.WriteInt32((int)Buff.BuffCategoryAndIDFromCategoryAndAttributeID(buff.Category, buff.AttributeID));
                writer.WriteInt32((int)buff.Mode);
                writer.WriteInt32(buff.Value);
            }
        }
    }
}
