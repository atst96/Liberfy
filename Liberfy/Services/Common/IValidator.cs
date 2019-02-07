using Liberfy.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Liberfy.Services
{
    internal interface IValidator
    {
        int MaxPostTextLength { get; }

        int CountTextLength(string text);

        bool CanPost(int textLength, ICollection<UploadMedia> sources);
    }
}
