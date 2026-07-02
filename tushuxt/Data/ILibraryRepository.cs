using System;
using System.Collections.Generic;

namespace tushuxt
{
    public interface ILibraryRepository
    {
        LibraryData Load();
        void Save(LibraryData data);
        
        // 可选：添加更细粒度的 CRUD 方法，但为了兼容现有代码，我们先保持 Load/Save
    }
}
