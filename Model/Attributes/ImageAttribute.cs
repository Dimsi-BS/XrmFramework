using System;

namespace Plugins
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class ImageAttribute : Attribute
    {
        public string[] Columns { get; private set; }

        public bool AllColumns { get; private set; }

        public ImageAttribute(params string[] columns)
        {
            Columns = columns;
            AllColumns = false;
        }

        public ImageAttribute(bool allColumns)
        {
            Columns = null;
            AllColumns = allColumns;
        }
    }

    public class PreImageAttribute : ImageAttribute
    {
        public PreImageAttribute(params string[] columns)
            : base(columns)
        {
        }
        public PreImageAttribute(bool allColumns)
            : base(allColumns)
        {
        }
    }

    public class PostImageAttribute : ImageAttribute
    {
        public PostImageAttribute(params string[] columns)
            : base(columns)
        {
        }
        public PostImageAttribute(bool allColumns)
            : base(allColumns)
        {
        }
    }
}