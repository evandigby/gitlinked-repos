using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client
{
    interface IState
    {
        bool IsAuthenticated { get; set; }
        IEnumerable<Repo> Repositories { get; set; }
        bool RepositoriesStale { get; set; }
        event Action OnChange;
        void NotifyStateChanged();
        IEnumerable<Repo> SelectedRepositories { get; set; }
        void AddRepository(params Repo[] toAdd);
        void RemoveRepository(params Repo[] toRemove);
    }
}
