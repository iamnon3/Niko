using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace Trak_IT.Scripts
{
    public class ProfileBind : BindableObject
    {

        // Event to notify that a property has changed

        private string _profileImageFilePath;

        public string ProfileImageFilePath
        {
            get => _profileImageFilePath;
            set
            {
                if(_profileImageFilePath != value)
                {
                    _profileImageFilePath = value;
                    OnPropertyChanged();
                }
            }
        }

    }
}
