using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstateAgency
{
    public interface IDialogService
    {
        void ShowMessage(string message); 
        string FilePath { get; set; } 
        bool OpenFileDialog(); 
        bool SaveFileDialog();
        bool QuestionToUser(string text);
    }
}
