﻿using ApiCatalogo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ApiCatalogo.DTOs;

public class ProdutoDTO
{
    public int ProdutoId { get; set; }
    [Required]
    [StringLength(80)]
    public string? Nome { get; set; }
    [Required]
    [StringLength(300)]
    public string? Descricao { get; set; }
    [Required]
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }
    public decimal Preco { get; set; }
    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; set; }
    public int CategoriaId { get; set; }
}

