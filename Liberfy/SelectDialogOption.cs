using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy
{
    internal class SelectDialogOption<T>
    {
        public string Instruction { get; set; }

        public string SelectedValuePath { get; set; }

        public string DisplayMemberPath { get; set; }

        public DataTemplate ItemTemplate { get; set; }

        public DataTemplateSelector ItemTemplateSelector { get; set; }

        public T SelectedItem { get; set; }

        public T SelectedValue { get; set; }

        public IEnumerable<T> Items { get; set; }

        public bool IsSelected { get; set; }
    }
}
