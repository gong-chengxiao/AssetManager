using AssetManager.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AssetManager.ViewModels;

public class HomeViewModel : ObservableRecipient
{
    private string _greetingMessage;

    public string GreetingMessage
    {
        get => _greetingMessage;
    }
    
    public HomeViewModel()
    {
        _greetingMessage = GetGreetingMessage();
    }

    private string GetGreetingMessage()
    {
        var hour = DateTime.Now.Hour;
        var greeting =    hour < 12 ? "Home_MorningGreeting".GetLocalized() 
                        : hour < 18 ?  "Home_AfternoonGreeting".GetLocalized()
                        : "Home_EveningGreeting".GetLocalized();
        return $"{greeting}, {Environment.UserName}!";
    }
}
