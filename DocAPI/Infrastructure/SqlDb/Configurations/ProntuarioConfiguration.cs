using DocAPI.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
//Procura todas classes que implementam IEntityTypeConfiguration<>
public class ProntuarioConfiguration : IEntityTypeConfiguration<Prontuario>
{
    //a interface exige o método montar o modelo EF
    public void Configure(EntityTypeBuilder<Prontuario> builder)
    {
        
    }
}