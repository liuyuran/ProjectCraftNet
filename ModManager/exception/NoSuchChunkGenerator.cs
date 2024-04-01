namespace ModManager.exception;

public class NoSuchChunkGenerator(long worldId) : BaseException($"No chunk generator found for world {worldId}");