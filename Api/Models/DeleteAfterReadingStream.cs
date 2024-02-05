using Microsoft.Win32.SafeHandles;

namespace Api.Models;

/// <summary>
/// Deletes the file during disposal
/// <see cref="FileStream" />
/// </summary>
public class DeleteAfterReadingStream : FileStream
{
    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (File.Exists(Name))
            File.Delete(Name);
    }

    /// <inheritdoc />
    public DeleteAfterReadingStream(SafeFileHandle handle, FileAccess access) : base(handle, access) { }

    /// <inheritdoc />
    public DeleteAfterReadingStream(SafeFileHandle handle, FileAccess access, int bufferSize) : base(handle, access, bufferSize) { }

    /// <inheritdoc />
    public DeleteAfterReadingStream(SafeFileHandle handle, FileAccess access, int bufferSize, bool isAsync) : base(handle, access, bufferSize, isAsync) { }

    /// <inheritdoc />
    [Obsolete]
    public DeleteAfterReadingStream(IntPtr handle, FileAccess access) : base(handle, access) { }

    /// <inheritdoc />
    [Obsolete]
    public DeleteAfterReadingStream(IntPtr handle, FileAccess access, bool ownsHandle) : base(handle, access, ownsHandle) { }

    /// <inheritdoc />
    [Obsolete]
    public DeleteAfterReadingStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize) : base(handle, access, ownsHandle, bufferSize) { }

    /// <inheritdoc />
    [Obsolete]
    public DeleteAfterReadingStream(IntPtr handle, FileAccess access, bool ownsHandle, int bufferSize, bool isAsync) : base(handle, access, ownsHandle, bufferSize, isAsync) { }

    /// <inheritdoc />
    public DeleteAfterReadingStream(string path, FileMode mode) : base(path, mode) { }

    /// <inheritdoc />
    public DeleteAfterReadingStream(string path, FileMode mode, FileAccess access) : base(path, mode, access) { }

    /// <inheritdoc />
    public DeleteAfterReadingStream(string path, FileMode mode, FileAccess access, FileShare share) : base(path, mode, access, share) { }

    /// <inheritdoc />
    public DeleteAfterReadingStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize) : base(path, mode, access, share, bufferSize) { }

    /// <inheritdoc />
    public DeleteAfterReadingStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync) : base(path, mode, access, share, bufferSize, useAsync) { }

    /// <inheritdoc />
    public DeleteAfterReadingStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, FileOptions options) : base(path, mode, access, share, bufferSize, options) { }
}