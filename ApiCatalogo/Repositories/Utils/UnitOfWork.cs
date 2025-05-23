﻿using ApiCatalogo.Context;
using ApiCatalogo.Repositories.Interfaces;

namespace ApiCatalogo.Repositories.Utils;

public class UnitOfWork : IUnitOfWork
{
    private IProdutoRepository? _produtoRepo;
    private ICategoriaRepository? _categoriaRepo;

    public AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IProdutoRepository ProdutoRepository
    {
        get
        {
            //return _produtoRepo = _produtoRepo ?? new ProdutoRepository(_context);
            if (_produtoRepo ==null)
            {
                _produtoRepo = new ProdutoRepository(_context);
            }
            return _produtoRepo;
        }
    }

    public ICategoriaRepository CategoriaRepository
    {
        get
        {
           return _categoriaRepo = _categoriaRepo ?? new CategoriaRepository(_context);
        }
    }

    public async Task CommitAsync()
    {
       await _context.SaveChangesAsync();
    }
}
