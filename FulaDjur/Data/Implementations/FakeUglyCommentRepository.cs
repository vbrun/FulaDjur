using FulaDjur.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FulaDjur.Data.Implementations
{
    public class FakeUglyCommentRepository : IUglyCommentRepository
    {
        public List<UglyCommentModel> GetAll(int id)
        {
            var allUglyComments = new List<UglyCommentModel>()
            {
                new UglyCommentModel { AnimalId = 1, Name = "Bjön", Text = "Va ful!!!"},
                new UglyCommentModel { AnimalId = 1, Name = "Pontus", Text = "OMG!"},
                new UglyCommentModel { AnimalId = 2, Name = "Vbrun", Text = "å fyfan"},
                new UglyCommentModel { AnimalId = 3, Name = "Jim", Text = "Ser ut som Björn"}
            };

            var uglyComments = allUglyComments.Where(comment => comment.AnimalId == id).ToList();

            return uglyComments;
        }


        public void Create(UglyCommentModel comment)
        {
            throw new NotImplementedException();
        }
    }
}
