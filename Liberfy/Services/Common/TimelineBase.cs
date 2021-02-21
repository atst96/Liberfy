using Liberfy.Managers;

namespace Liberfy
{
    internal abstract class TimelineBase : NotificationObject
    {
        public static ColumnManageer Columns { get; } = App.Columns;

        protected ColumnManager AccountColumns { get; }

        protected TimelineBase(IAccount account)
        {
            this.AccountColumns = new ColumnManager(account, App.Columns);
        }

        public abstract void Load();

        public abstract void Unload();
    }
}
