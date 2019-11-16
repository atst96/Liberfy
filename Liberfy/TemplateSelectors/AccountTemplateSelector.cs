using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Liberfy.TemplateSelectors
{
    internal class AccountTemplateSelector : DataTemplateSelector
    {
        private App _app { get; } = App.Instance;

        private readonly Dictionary<string, DataTemplate> _templates = new Dictionary<string, DataTemplate>();

        private DataTemplate GetTemplate(string templateName)
        {
            if (this._templates.TryGetValue(templateName, out var template))
            {
                return template;
            }

            template = this._app.TryFindResource<DataTemplate>(templateName);

            this._templates.Add(templateName, template);

            return template;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return item switch
            {
                TwitterAccount _ => this.GetTemplate("AccountTemplate.Twitter"),
                MastodonAccount _ => this.GetTemplate("AccountTemplate.Mastodon"),
                _ => base.SelectTemplate(item, container),
            };
        }
    }
}
