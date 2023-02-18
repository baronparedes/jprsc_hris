using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.Data
{
    internal class TableBuilder<TItem> where TItem : class, new()
    {
        internal TableRowBuilder<TItem> _headerOneBuilder = new TableRowBuilder<TItem>();
        internal TableRowBuilder<TItem> _headerTwoBuilder = new TableRowBuilder<TItem>();
        internal TableRowBuilder<TItem> _rowBuilder = new TableRowBuilder<TItem>();
        internal TableRowAggregateBuilder<TItem> _footerBuilder = new TableRowAggregateBuilder<TItem>();
        private bool _hasFooter = false;

        /// <summary>
        /// Sets up a column for the resulting table.
        /// </summary>
        /// <param name="header">The column name that will appear on the first row of the table.</param>
        /// <param name="value">The method to use for getting the value out of the item.</param>
        /// <param name="footer">The method to use for getting the value that will appear in the footer.</param>
        internal void Column(string header, Func<TItem, string> value, Func<IList<TItem>, string> footer = null)
        {
            _headerOneBuilder.Add(header);
            _rowBuilder.Add(value);
            if (footer != null)
            {
                _hasFooter = true;
                _footerBuilder.Add(footer);
            }
            else
            {
                _footerBuilder.Add(String.Empty);
            }
        }

        /// <summary>
        /// Sets up a column for the resulting table.
        /// </summary>
        /// <param name="headerOnFirstLine">The column name that will appear on the first row of the table.</param>
        /// <param name="headerOnSecondLine">The column name that will appear on the second row of the table.</param>
        /// <param name="value">The method to use for getting the value out of the item.</param>
        /// <param name="footer">The method to use for getting the value that will appear in the footer.</param>
        internal void Column(string headerOnFirstLine, string headerOnSecondLine, Func<TItem, string> value, Func<IList<TItem>, string> footer = null)
        {
            Column(headerOnFirstLine, value, footer);
            _headerTwoBuilder.Add(headerOnSecondLine);
        }

        /// <summary>
        /// Builds the table using the given items.
        /// </summary>
        internal IList<IList<string>> Build(IList<TItem> items)
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
                if (items != null && items.Any())
                {
                    foreach (var row in items)
                    {
                        lines.Add(_rowBuilder.Build(row));
                    }
                }
            }

            if (_hasFooter && _footerBuilder != null && _footerBuilder.HasItems())
            {
                if (items != null && items.Any())
                {
                    lines.Add(_footerBuilder.Build(items));
                }
            }

            return lines;
        }
    }

    internal class TableRowBuilder<TItem> where TItem : class, new()
    {
        private IList<Func<TItem, string>> _valueFactory = new List<Func<TItem, string>>();

        internal void Add(string value)
        {
            _valueFactory.Add(item => value);
        }

        internal void Add(Func<TItem, string> valueFactory)
        {
            _valueFactory.Add(valueFactory);
        }

        internal IList<string> Build()
        {
            var item = new TItem();
            return Build(item);
        }

        internal IList<string> Build(TItem item)
        {
            var values = new List<string>();
            foreach (var factory in _valueFactory)
            {
                values.Add(factory(item));
            }
            return values;
        }

        internal bool HasItems()
        {
            return _valueFactory.Count > 0;
        }
    }

    internal class TableRowAggregateBuilder<TItem> where TItem : class, new()
    {
        private IList<Func<IList<TItem>, string>> _valueFactory = new List<Func<IList<TItem>, string>>();

        internal void Add(string value)
        {
            _valueFactory.Add(item => value);
        }

        internal void Add(Func<IList<TItem>, string> valueFactory)
        {
            _valueFactory.Add(valueFactory);
        }

        internal IList<string> Build()
        {
            var items = new List<TItem>();
            return Build(items);
        }

        internal IList<string> Build(IList<TItem> items)
        {
            var values = new List<string>();
            foreach (var factory in _valueFactory)
            {
                values.Add(factory(items));
            }
            return values;
        }

        internal bool HasItems()
        {
            return _valueFactory.Count > 0;
        }
    }
}
