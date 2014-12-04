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
        public List<UglyCommentModel> GetAll(string id)
        {
            var allUglyComments = new List<UglyCommentModel>()
            {
                new UglyCommentModel { AnimalId = "1", Name = "Bjön", Text = "Va ful!!!", Created = DateTime.Now.AddHours(-5)},
                new UglyCommentModel { AnimalId = "1", Name = "Pontus", Text = "OMG!", Created = DateTime.Now.AddHours(-3)},
                new UglyCommentModel { AnimalId = "2", Name = "Vbrun", Text = "å fyfan", Created = DateTime.Now.AddHours(-2)},
                new UglyCommentModel { AnimalId = "3", Name = "Jim", Text = "Ser ut som Björn", Created = DateTime.Now.AddHours(-1)}
            };

            var uglyComments = allUglyComments.Where(comment => comment.AnimalId == id).ToList();

            return uglyComments;
        }


        public void Create(UglyCommentModel comment)
        {
            throw new NotImplementedException();
        }


        public void Delete(string ImageUri)
        {
            throw new NotImplementedException();
        }
    }
}
