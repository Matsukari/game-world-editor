using Nez;

namespace Raven
{
  public class SelectionPoints 
  {
    public List<RectangleF> Points = new List<RectangleF>();
    public int Radius = 5;
    public int SafeBuffer = 3;

    public SelectionPoints()
    {
      for (int i = 0; i < 8; i++)
      {
        Points.Add(new RectangleF(0, 0, Radius, Radius));
      }
    }

    public void Update(RectangleF area)
    {
      Points.Clear();
      Points.Add(new RectangleF(area.Left , area.Top         , Radius, Radius));
      Points.Add(new RectangleF(area.Left , area.Bottom      , Radius, Radius));
      Points.Add(new RectangleF(area.Left , area.Center.Y    , Radius, Radius));
      Points.Add(new RectangleF(area.Right , area.Top        , Radius, Radius));
      Points.Add(new RectangleF(area.Right , area.Bottom     , Radius, Radius));
      Points.Add(new RectangleF(area.Right , area.Center.Y   , Radius, Radius));
      Points.Add(new RectangleF(area.Center.X , area.Top     , Radius, Radius));
      Points.Add(new RectangleF(area.Center.X , area.Bottom  , Radius, Radius)); 
    } 
  }
}
