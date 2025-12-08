namespace Vergil;

public class Tag(IRepository repository, Reference reference) : ReferencePointer<GitObject>(repository, reference);
