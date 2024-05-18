using ImGuiNET;
using System.Numerics;
using Microsoft.Xna.Framework.Graphics;
using Nez;
using Mono = Microsoft.Xna.Framework;

namespace Raven 
{
  public class ImGuiUtils
  {
    public static Vector2 CalcTextSizeHorizontal(params string[] texts)
    {
      Vector2 size = Vector2.Zero;
      foreach (var text in texts)
      {
        size.X += ImGui.CalcTextSize(text).X;
        size.Y = Math.Max(ImGui.CalcTextSize(text).Y, size.Y);
      }
      return size;
    }
    public static bool DisabledButtonIf(bool condition, string label)
    {
      if (condition)
        ImGui.BeginDisabled();

      var result = false;
      if (ImGui.Button(label)) result = true;

      if (condition)
        ImGui.EndDisabled();

      return result;
    }
    public static bool FillButtonX(string label)
    {
      var button = ImGui.Button(label, new Vector2(ImGui.GetContentRegionAvail().X, Nez.ImGuiTools.NezImGui.GetDefaultWidgetHeight()));
      return button;
    }
    public static bool MiddleButtonX(string name)
    {
      return ImGui.Button(name);
    }
    public static void DisableIf(bool condition, Action content)
    {
      if (condition)
        ImGui.BeginDisabled();

      content.Invoke();

      if (condition)
        ImGui.EndDisabled();
    }
    public static void TabItem(string label, Action content)
    {
      if (ImGui.BeginTabItem(label))
      {
        content.Invoke();
        ImGui.EndTabItem();
      }
    }
    public static bool MenuItem(string icon, string label) => ImGui.MenuItem(icon + "   " + label);


    public static void SpanX(float x)
    {
      ImGui.SameLine();
      ImGui.Dummy(new System.Numerics.Vector2(x, 0f));
      ImGui.SameLine();
    }

    public static Vector2 Rotate(Vector2 v, float cos_a, float sin_a) => new Vector2(v.X * cos_a - v.Y * sin_a, v.X * sin_a + v.Y * cos_a);

    public static Vector2 ContainSize(Vector2 size, Vector2 intendedSize)
    {
        float imageRatio = 1f;
        if (size.X > size.Y) 
          imageRatio = intendedSize.X / size.X;
        else 
          imageRatio = intendedSize.Y / size.Y;

        return size * imageRatio;
    }

    public static void TextMiddle(string hint)
    {
      // Calculate the size of the child window
      var childSize = ImGui.GetWindowSize();

      var textPos = new System.Numerics.Vector2();
      textPos.X = (childSize.X - ImGui.CalcTextSize(hint).X) * 0.5f;
      textPos.Y = (childSize.Y - ImGui.GetTextLineHeightWithSpacing()) * 0.5f;

      ImGui.SetCursorPos(textPos);
      ImGui.TextDisabled(hint);
    }
    public static void TextMiddleX(string hint)
    {
      // Calculate the size of the child window
      var childSize = ImGui.GetWindowSize();

      var textPos = new System.Numerics.Vector2();
      textPos.X = (childSize.X - ImGui.CalcTextSize(hint).X) * 0.5f;

      ImGui.SetCursorPos(textPos);
      ImGui.Text(hint);
    }

    public static void DrawRect(ImDrawListPtr drawList, RectangleF region, Mono.Color color)
    {
      ImGui.GetBackgroundDrawList().AddRectFilled(region.Location.ToNumerics(), region.Max.ToNumerics(), color.ToImColor()); 
    }

    public static void DrawEllipse(ImDrawListPtr drawList, RectangleF bounds, uint color)
    {
      float center_x = bounds.Center.X;
      float center_y = bounds.Center.Y;
      float radius_x = bounds.Width/2;
      float radius_y = bounds.Height/2;
      float angle = 0.0f;
      float angle_increment = 0.1f;
      int num_segments = 360;
      float last_x = 0;
      float last_y = 0;
      for (int i = 0; i < num_segments; i++) {
        float angle_i = angle + i * angle_increment;
        float x = center_x + radius_x * MathF.Cos(angle_i);
        float y = center_y + radius_y * MathF.Sin(angle_i);
        if (i == 0) {
          // drawList.AddLine(new Vector2(center_x, center_y), new Vector2(x, y), color);
        } else {
          drawList.AddLine(new Vector2(last_x, last_y), new Vector2(x, y), color);
        }
        last_x = x;
        last_y = y;
      }
    }
    public static void DrawEllipseFilled(ImDrawListPtr draw_list, RectangleF bounds, uint col, int num_segments=360)
    {
      // Draw a filled ellipse
      Vector2 center = bounds.Center.ToNumerics();
      float radiusX = bounds.Width/2;
      float radiusY = bounds.Height/2;

      // Draw filled ellipse approximation
      Vector2 prev_point = new Vector2(center.X + radiusX, center.Y);
      for (int i = 1; i <= num_segments; ++i) {
        float angle = ((float)i / (float)num_segments) * 2.0f * MathF.PI;
        Vector2 point = new Vector2(center.X + radiusX * MathF.Cos(angle), center.Y + radiusY * MathF.Sin(angle));
        draw_list.AddTriangleFilled(center, prev_point, point, col);
        prev_point = point;
      }
    }
    public static void DrawImageContained(Texture2D texture, float height)
    {
      var imageSize = ImGuiUtils.ContainSize(texture.GetSize().ToNumerics(), new Vector2(ImGui.GetContentRegionAvail().X, height));
      var textureId = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(texture);
      ImGui.Image(textureId, new System.Numerics.Vector2(imageSize.X, imageSize.Y));
    }
    public static void DrawImageContained(Texture2D texture, Vector2 intendedSize)
    {
      var imageSize = ImGuiUtils.ContainSize(texture.GetSize().ToNumerics(), intendedSize);
      var textureId = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(texture);
      ImGui.Image(textureId, new System.Numerics.Vector2(imageSize.X, imageSize.Y));
    }
    public static void DrawImage(Sprite sprite, Vector2 size)
    {
      nint texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(sprite.Texture);
      ImGui.Image(texture, size, sprite.MinUv.ToNumerics(), sprite.MaxUv.ToNumerics());
    }
    public static void DrawImage(ImDrawListPtr drawList, Sprite sprite, Vector2 pos, Vector2 size, Mono.Color color = default)
    {
      if (color == default) color = Mono.Color.White;
      DrawImage(drawList, sprite.Texture, sprite.Region, pos, size, Vector2.Zero, 0f, color.ToImColor());
    }
    public static void DrawImage(ImDrawListPtr drawList, Texture2D texture, Vector2 pos, Vector2 size, Mono.Color color = default)
    {
      if (color == default) color = Mono.Color.White;
      DrawImage(drawList, texture, new Mono.Rectangle(0, 0, texture.Width, texture.Height), pos, size, Vector2.Zero, 0f, color.ToImColor());
    }
    public static void DrawImageCentered(ImDrawListPtr drawList, Texture2D tx2, Vector2 center, Vector2 size)
    {
      Vector2[] pos = 
      {
        center + new Vector2(-size.X * 0.5f, -size.Y * 0.5f),
        center + new Vector2(+size.X * 0.5f, -size.Y * 0.5f),
        center + new Vector2(+size.X * 0.5f, +size.Y * 0.5f),
        center + new Vector2(-size.X * 0.5f, +size.Y * 0.5f)
      };
      Vector2[] uvs = 
      { 
        new Vector2(0.0f, 0.0f), 
        new Vector2(1.0f, 0.0f), 
        new Vector2(1.0f, 1.0f), 
        new Vector2(0.0f, 1.0f) 
      };
      nint texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(tx2);
      drawList.AddImageQuad(texture, pos[0], pos[1], pos[2], pos[3], uvs[0], uvs[1], uvs[2], uvs[3], ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 1)));
    }
    public static void DrawImage(ImDrawListPtr drawList, Texture2D tx2, Mono.Rectangle region, Vector2 position, 
        Vector2 size, Vector2 origin, float angle, uint color)
    {
      nint texture = Core.GetGlobalManager<Nez.ImGuiTools.ImGuiManager>().BindTexture(tx2);
      float cos_a = MathF.Cos(angle);
      float sin_a = MathF.Sin(angle);

      position += origin;

      Vector2[] pos = new []
      {
        position + Rotate(new Vector2(0f, 0f), cos_a, sin_a),
        position + Rotate(new Vector2(size.X, 0f), cos_a, sin_a),
        position + Rotate(new Vector2(size.X, size.Y), cos_a, sin_a),
        position + Rotate(new Vector2(0f, size.Y), cos_a, sin_a)
      };
      var texSize = tx2.GetSize();
      Vector2[] uvs = new []
      { 
        new Vector2(region.X/texSize.X, region.Y/texSize.Y), 
        new Vector2(region.Right/texSize.X, region.Y/texSize.Y), 
        new Vector2(region.Right/texSize.X, region.Bottom/texSize.Y), 
        new Vector2(region.X/texSize.X, region.Bottom/texSize.Y) 
      };

      
      drawList.AddImageQuad(texture, pos[0], pos[1], pos[2], pos[3], uvs[0], uvs[1], uvs[2], uvs[3], color);
    }

  }
}
