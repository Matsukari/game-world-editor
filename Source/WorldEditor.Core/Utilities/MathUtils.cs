

namespace Raven
{
  static class MathUtils
  {
    public static void Modulo(ref int x, int m) => x = (x % m + m) % m;
  }
}
