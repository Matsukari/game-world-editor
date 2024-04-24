
A single action cannot execute Do or Undo consecutively; must be alternately called


interface EditorAction
{
	void Do();
	void Undo();
}

class Rotate : EditorAction {}
class Scale : EditorAction {}
class ChangeProperty : EditorAction {}
class AddSpritex : EditorAction 
{
  Spritex spritex;
  public AddSpritex(ref spritex)
  {
    spritex = spritex;
  }

	void Do()
	{
    sheet.tryadd(spritex);  
	}
  void Undo()
  {
    sheet.remove(spritex);
  }

}






class EditorActionManager
{
  array<EditorAction> actions;
  EditorAction current;

  void undo()
  {
    current.undo();
    current--;
  }
  void redo()
  {
    current++;
    current.do();
  }
  void do(action)
  {
    remove_all_after_current();
    actions.add(action);
    current++;
  }
}
class Move : EditorAction
{
  Vector2 position;
  Vector2 lastPosition;

  public Move(ref whose = camera)
  {
    lastPosition = camera.position;
  }

	void Do()
	{
    camera.position = position;
	}
  void Undo()
  {
    camera.position = lastPosition;
  }

}



static void main()
{
  EditorActionManager history;

  history.do(Rotate(sprite, 32)); // 0
  history.do(Rotate(sprite, 50)); // 1
  history.do(Move(sprite, 50)); // 2
  // history.do(ChangeProperty(sprite, name)); // 3

  history.undo(); // undo Move

  // Action left: 0, 1
  // In rotate index 1 
  
  history.do(DeleteProperty(sprite, name)); // gone to index 2 
  // Action left: 0, 1, 2

}



