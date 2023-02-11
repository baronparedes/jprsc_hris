using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.Data
{
    public class TableBuilder<T> where T : class, new()
    {
        public IList<IList<string>> Lines { get; private set; } = new List<IList<string>>();
        public TableRowBuilder<T> HeaderOneBuilder { get; private set; } = new TableRowBuilder<T>();
        public TableRowBuilder<T> HeaderTwoBuilder { get; private set; } = new TableRowBuilder<T>();
        public TableRowBuilder<T> RowBuilder { get; private set; } = new TableRowBuilder<T>();

        public void Column(string headerOne, Func<T, string> value)
        {
            HeaderOneBuilder.Add(headerOne);
            RowBuilder.Add(value);
        }

        public void Column(string headerOne, string headerTwo, Func<T, string> value)
        {
            Column(headerOne, value);
            HeaderTwoBuilder.Add(headerTwo);
        }

        public IList<IList<string>> Build(IEnumerable<T> rows)
        {
            var lines = new List<IList<string>>();

            if (HeaderOneBuilder != null)
            {
                lines.Add(HeaderOneBuilder.Build());
            }

            if (HeaderTwoBuilder != null)
            {
                lines.Add(HeaderTwoBuilder.Build());
            }

            if (RowBuilder != null)
            {
                if (rows != null && rows.Any())
                {
                    foreach (var row in rows)
                    {
                        lines.Add(RowBuilder.Build(row));
                    }
                }
            }

            return lines;
        }
    }

    public class TableRowBuilder<T> where T : class, new()
    {
        private IList<Func<T, string>> _valueFactory = new List<Func<T, string>>();

        public void Add(string value)
        {
            _valueFactory.Add(data => value);
        }

        public void Add(Func<T, string> valueFactory)
        {
            _valueFactory.Add(valueFactory);
        }

        public IList<string> Build()
        {
            var data = new T();
            return Build(data);
        }

        public IList<string> Build(T data)
        {
            var values = new List<string>();
            foreach (var factory in _valueFactory)
            {
                values.Add(factory(data));
            }
            return values;
        }
    }
}
