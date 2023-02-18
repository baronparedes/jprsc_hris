using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.Data
{
    public class TableBuilder<T> where T : class, new()
    {
        public TableRowBuilder<T> _headerOneBuilder = new TableRowBuilder<T>();
        public TableRowBuilder<T> _headerTwoBuilder = new TableRowBuilder<T>();
        public TableRowBuilder<T> _rowBuilder = new TableRowBuilder<T>();
        private Func<IList<T>, IList<string>> _footerValuesFactory;

        public void Column(string headerOne, Func<T, string> value)
        {
            _headerOneBuilder.Add(headerOne);
            _rowBuilder.Add(value);
        }

        public void Column(string headerOne, string headerTwo, Func<T, string> value)
        {
            Column(headerOne, value);
            _headerTwoBuilder.Add(headerTwo);
        }

        public void Footer(Func<IList<T>, IList<string>> valuesFactory)
        {
            _footerValuesFactory = valuesFactory;
        }

        public IList<IList<string>> Build(IList<T> rows)
        {
            var lines = new List<IList<string>>();

            if (_headerOneBuilder != null && _headerOneBuilder.HasItems())
            {
                lines.Add(_headerOneBuilder.Build());
            }

            if (_headerTwoBuilder != null && _headerTwoBuilder.HasItems())
            {
                lines.Add(_headerTwoBuilder.Build());
            }

            if (_rowBuilder != null && _rowBuilder.HasItems())
            {
                if (rows != null && rows.Any())
                {
                    foreach (var row in rows)
                    {
                        lines.Add(_rowBuilder.Build(row));
                    }
                }
            }

            if (_footerValuesFactory != null)
            {
                if (rows != null && rows.Any())
                {
                    lines.Add(_footerValuesFactory(rows));
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

        public bool HasItems()
        {
            return _valueFactory.Count > 0;
        }
    }
}
