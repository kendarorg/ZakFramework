namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	internal enum ConcurrentTreeOperationTypes
	{
		MsgFindByPath,
		MsgGetChildren,
		MsgAddChild,
		MsgRemoveChild,
		MsgRemoveChildByName,
		MsgRename
	}
}