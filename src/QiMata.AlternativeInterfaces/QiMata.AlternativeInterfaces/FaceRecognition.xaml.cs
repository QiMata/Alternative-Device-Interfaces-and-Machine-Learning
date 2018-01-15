using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace QiMata.AlternativeInterfaces
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FaceRecognition : ContentPage
    {
        public FaceRecognition()
        {
            InitializeComponent();
        }
    }
}