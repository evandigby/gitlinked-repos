using BlazorApp.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Client
{
    public class State : IState
    {
        public bool IsAuthenticated { get; set; }

        public IEnumerable<Repo> Repositories { get; set; } = Enumerable.Empty<Repo>();
        public IEnumerable<Repo> SelectedRepositories { get; set; } = Enumerable.Empty<Repo>();

        public void AddRepository(params Repo[] toAdd)
        {
            SelectedRepositories = SelectedRepositories.Concat(toAdd);
            NotifyStateChanged();
        }

        public void RemoveRepository(params Repo[] toRemove)
        {
            SelectedRepositories = SelectedRepositories.Where(r => toRemove.Any(cr => cr.Id == r.Id));
            NotifyStateChanged();
        }

        public bool RepositoriesStale { get; set; }

        public event Action OnChange;
        
        // Don't auto-trigger to allow for batch updates of state
        public void NotifyStateChanged() => OnChange?.Invoke();
    }
}
