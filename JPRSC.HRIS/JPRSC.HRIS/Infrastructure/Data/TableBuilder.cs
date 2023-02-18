using System;
using System.Collections.Generic;
using System.Linq;

namespace JPRSC.HRIS.Infrastructure.Data
{
    public class TableBuilder<TItem> where TItem : class, new()
    {
        public TableRowBuilder<TItem> _headerOneBuilder = new TableRowBuilder<TItem>();
        public TableRowBuilder<TItem> _headerTwoBuilder = new TableRowBuilder<TItem>();
        public TableRowBuilder<TItem> _rowBuilder = new TableRowBuilder<TItem>();
        public TableRowAggregateBuilder<TItem> _footerBuilder = new TableRowAggregateBuilder<TItem>();
        private bool _hasFooter = false;
        private string _title;

        /// <summary>
        /// Sets the title of the table.
        /// </summary>
        public void Title(string title)
        {
            _title = title;
        }

        /// <summary>
        /// Sets up a column for the resulting table.
        /// </summary>
        /// <param name="header">The column name that will appear on the first row of the table.</param>
        /// <param name="value">The method to use for getting the value out of the item.</param>
        /// <param name="footer">The method to use for getting the value that will appear in the footer.</param>
        public void Column(string header, Func<TItem, string> value, Func<IEnumerable<TItem>, string> footer = null)
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
        public void Column(string headerOnFirstLine, string headerOnSecondLine, Func<TItem, string> value, Func<IEnumerable<TItem>, string> footer = null)
        {
            Column(headerOnFirstLine, value, footer);
            _headerTwoBuilder.Add(headerOnSecondLine);
        }

        /// <summary>
        /// Builds the table using the given items.
        /// </summary>
        public Table Build(IEnumerable<TItem> items)
        {
            IList<string> headerOne = null;
            if (_headerOneBuilder != null && _headerOneBuilder.HasItems())
            {
                headerOne = _headerOneBuilder.Build();
            }

            IList<string> headerTwo = null;
            if (_headerTwoBuilder != null && _headerTwoBuilder.HasItems())
            {
                headerTwo = _headerTwoBuilder.Build();
            }

            IList<IList<string>> body = null;
            if (_rowBuilder != null && _rowBuilder.HasItems())
            {
                if (items != null && items.Any())
                {
                    body = new List<IList<string>>();
                    foreach (var row in items)
                    {
                        body.Add(_rowBuilder.Build(row));
                    }
                }
            }

            IList<string> footer = null;
            if (_hasFooter && _footerBuilder != null && _footerBuilder.HasItems())
            {
                if (items != null && items.Any())
                {
                    footer = _footerBuilder.Build(items);
                }
            }

            return new Table(_title, headerOne, headerTwo, body, footer);
        }
    }

    public class Table
    {
        public Table(string title, IList<string> headerOne, IList<string> headerTwo, IList<IList<string>> body, IList<string> footer)
        {
            Title = title;
            HeaderOne = headerOne;
            HeaderTwo = headerTwo;
            Body = body;
            Footer = footer;

            var lines = new List<IList<string>>();
            if (HeaderOne != null && HeaderOne.Count > 0) lines.Add(HeaderOne);
            if (HeaderTwo != null && HeaderTwo.Count > 0) lines.Add(HeaderTwo);
            if (Body != null && Body.Count > 0) lines.AddRange(Body);
            if (Footer != null && Footer.Count > 0) lines.Add(Footer);
            AllLines = lines;
        }

        public string Title { get; private set; }
        public IList<string> HeaderOne { get; private set; }
        public IList<string> HeaderTwo { get; private set; }
        public IList<IList<string>> Body { get; private set; }
        public IList<string> Footer { get; private set; }
        public IList<IList<string>> AllLines { get; private set; }
    }

    public class TableRowBuilder<TItem> where TItem : class, new()
    {
        private IList<Func<TItem, string>> _valueFactory = new List<Func<TItem, string>>();

        public void Add(string value)
        {
            _valueFactory.Add(item => value);
        }

        public void Add(Func<TItem, string> valueFactory)
        {
            _valueFactory.Add(valueFactory);
        }

        public IList<string> Build()
        {
            var item = new TItem();
            return Build(item);
        }

        public IList<string> Build(TItem item)
        {
            var values = new List<string>();
            foreach (var factory in _valueFactory)
            {
                values.Add(factory(item));
            }
            return values;
        }

        public bool HasItems()
        {
            return _valueFactory.Count > 0;
        }
    }

    public class TableRowAggregateBuilder<TItem> where TItem : class, new()
    {
        private IList<Func<IEnumerable<TItem>, string>> _valueFactory = new List<Func<IEnumerable<TItem>, string>>();

        public void Add(string value)
        {
            _valueFactory.Add(item => value);
        }

        public void Add(Func<IEnumerable<TItem>, string> valueFactory)
        {
            _valueFactory.Add(valueFactory);
        }

        public IList<string> Build()
        {
            var items = new List<TItem>();
            return Build(items);
        }

        public IList<string> Build(IEnumerable<TItem> items)
        {
            var values = new List<string>();
            foreach (var factory in _valueFactory)
            {
                values.Add(factory(items));
            }
            return values;
        }

        public bool HasItems()
        {
            return _valueFactory.Count > 0;
        }
    }
}
