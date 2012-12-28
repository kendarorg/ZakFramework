namespace _003AConcurrentTreeStructure.Lib.ConcurrentTreeInternals
{
	internal enum ConcurrentTreeMessageTypes
	{
		MsgFindByPath,
		MsgGetChildren,
		MsgAddChild,
		MsgRemoveChild,
		MsgRemoveChildByName,
		MsgRename
	}
}