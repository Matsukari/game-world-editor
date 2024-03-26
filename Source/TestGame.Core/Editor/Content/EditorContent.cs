
namespace Raven.Sheet
{
  public class EditorContent
  {
    public EditorTabData Data = new EditorTabData();
    public IPropertied Content;
    public EditorContent(IPropertied content) => Content = content;
  }
}
