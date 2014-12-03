using FulaDjur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FulaDjur.Data
{
    public interface IUglyCommentRepository
    {
        List<UglyCommentModel> GetAll(int id);
        void Create(UglyCommentModel comment);
    }
}
