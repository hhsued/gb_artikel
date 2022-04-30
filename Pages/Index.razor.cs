using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using Syncfusion.Blazor.RichTextEditor;
using System.Text.RegularExpressions;

namespace Gemeindebriefbeitrag.Pages
{
  public partial class Index : ComponentBase
  {
    // Properties definition
    private Article _article = new Article();
    private string[] _availablePublishingRights = { "gedruckter Gemeindebrief", "digitaler Gemeindebrief", "Internetseite der Gemeinde", "Internetseite des Bezirks", "Internetseite der Gebietskirche", "ohne Einschränkungen" };
    private string value { get; set; } = "Nothing selected";
    private IEnumerable<string> _selectedRights { get; set; } = new HashSet<string>() { "gedruckter Gemeindebrief", "digitaler Gemeindebrief" };
    private bool _senderHasError = true;
    IList<IBrowserFile> files = new List<IBrowserFile>();

    // WYSIVIG-Editor Toolbar
    private List<ToolbarItemModel> Tools = new List<ToolbarItemModel>()
    {
        new ToolbarItemModel() { Command = ToolbarCommand.Bold },
        new ToolbarItemModel() { Command = ToolbarCommand.Italic },
        new ToolbarItemModel() { Command = ToolbarCommand.Underline },
        new ToolbarItemModel() { Command = ToolbarCommand.Undo },
        new ToolbarItemModel() { Command = ToolbarCommand.Redo }
    };

    // On-Events
    private void onUploadAlternativeFiles(InputFileChangeEventArgs e)
    {
      foreach (var file in e.GetMultipleFiles())
      {
        files.Add(file);
      }
    }
    private void onUploadFiles(InputFileChangeEventArgs e)
    {
      foreach (var file in e.GetMultipleFiles())
      {
        _article.Photos.Add(new Photo() { File = file });
      }
    }
    private void onSendArticle()
    {
      foreach(var file in _article.Photos)
      {
        file.File.OpenReadStream().
      }
      SendData.Send(_article);
      bool hasErrors = false;
      if (_article.Congregation == string.Empty)
      {
        Snackbar.Add("Bitte ein Ziel für den Artikel (Gemeinde, Bezirk) auswählen!", Severity.Error);
        return;
      }
      if (_article.Sender.Length == 0)
      {
        Snackbar.Add("Es wurde kein Absender angegeben!", Severity.Error);
        hasErrors = true;
      }
      Regex regEx = new Regex(@"[a-zA-Z0-9]{1,}@[a-zA-Z0-9]{1,}.[a-zA-Z]{1,}");
      if (_article.Sender.Length > 0 && !regEx.IsMatch(_article.Sender))
      {
        Snackbar.Add("Die Absenderadresse ist keine korrekte EMail-Adresse", Severity.Error);
        hasErrors = true;
      }
      if (_article.Headline.Length == 0)
      {
        Snackbar.Add("Der Artikel hat keine Überschrift", Severity.Error);
        hasErrors = true;
      }
      if (_article.Text.Length == 0)
      {
        Snackbar.Add("Der Artikel hat keinen Inhalt", Severity.Error);
        hasErrors = true;
      }
      if (_article.PublishingRights.Count() == 0)
      {
        Snackbar.Add("Für den Artikel müssen Rechte zur Veröffentlichung gesetzt werden!", Severity.Error);
        hasErrors = true;
      }
      foreach (var _photo in _article.Photos)
      {
        if (_photo.PublishingRights.Count() == 0)
        {
          Snackbar.Add("Für das Foto '" + _photo.File + "' müssen Rechte zur Veröffentlichung gesetzt werden!", Severity.Error);
          hasErrors = true;
        }
        if (_photo.Photographer == string.Empty)
        {
          Snackbar.Add("Für das Foto '" + _photo.File + "' muss ein Fotograf gesetzt werden!", Severity.Error);
          hasErrors = true;
        }
      }
      if (!hasErrors)
      {
      }
    }
    private void onCheckSenderError()
    {
      Regex regEx = new Regex(@"[a-zA-Z0-9]{1,}@[a-zA-Z0-9]{1,}.[a-zA-Z]{1,}");
      if (_article.Sender.Length > 3 && regEx.IsMatch(_article.Sender))
      {
        _senderHasError = false;
      }
    }
    private async Task onRemovePhoto(int index)
    {
      bool? result = await DialogService.ShowMessageBox("Hinweis", "Möchtest Du das Bild wirklich löschen?", yesText: "Ja", cancelText: "Nein");
      if (result != null)
      {
        _article.Photos.RemoveAt(index);
      }
      StateHasChanged();
    }

    private void onCongregationChanged(string congregation)
    {
      _article.Congregation = congregation;
    }
  }
  // ---------------------- Models
  class Article
  {
    public string Notes { get; set; } = string.Empty;
    public string Sender { get; set; } = string.Empty;
    public string Congregation { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public IEnumerable<string> PublishingRights { get; set; } = new HashSet<string>() { "gedruckter Gemeindebrief", "digitaler Gemeindebrief" };
    public List<Photo> Photos { get; set; } = new List<Photo>();
  }
  class Photo
  {
    public string Photographer { get; set; } = string.Empty;
    public string Caption { get; set; } = string.Empty;
    public string Hints { get; set; } = string.Empty;
    public IBrowserFile? File { get; set; } = null;
    public IEnumerable<string> PublishingRights { get; set; } = new HashSet<string>() { "gedruckter Gemeindebrief", "digitaler Gemeindebrief" };
  }
  static class Mode
  {
    public static bool IsImplementd { get; set; } = false;
  }

}
