using Dota2Helper.ViewModels;

namespace Dota2Helper.Design;

public class DesignSplashScreenViewModel : SplashScreenViewModel
{
    public DesignSplashScreenViewModel() : base(null!, null!)
    {
        StatusText = "GSI installed successfully";
    }
}