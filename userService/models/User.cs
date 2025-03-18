using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace userService.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } // Identificador único (UUID)

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Nombre del usuario

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } // Email único

        [Required]
        public string Password { get; set; }

        public bool EmailConfirmed { get; set; } // Contraseña (hash)

        public string  ConfirmationToken { get; set; }

        // Relación con DeviceUserRelation (1 usuario puede estar relacionado con múltiples dispositivos)
        [JsonIgnore]
        public ICollection<DeviceUserRelation> DeviceUserRelations { get; set; }

        // Relación con RelationUser (relaciones entre usuarios)
        [JsonIgnore]
        public ICollection<RelationUser> RelationUsersAsUser1 { get; set; }
        [JsonIgnore]
        public ICollection<RelationUser> RelationUsersAsUser2 { get; set; }
    }

    public class Device
    {
        [Key]
        public Guid Id { get; set; } // Identificador único (UUID)

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Nombre del dispositivo

        // Relación con DeviceUserRelation
        [JsonIgnore]
        public ICollection<DeviceUserRelation> DeviceUserRelations { get; set; }
    }

    public class DeviceUserRelation
{
    [Key]
    public Guid Id { get; set; } // Identificador único (UUID)

    // Clave foránea hacia User
    [Required]
    public Guid UserId { get; set; }

    public User? User { get; set; } // Propiedad de navegación (optional)

    // Clave foránea hacia Device
    [Required]
    public Guid DeviceId { get; set; }

    public Device? Device { get; set; } // Propiedad de navegación (optional)
}

    public class RelationUser
    {
        [Key]
        public Guid Id { get; set; } // Identificador único (UUID)

        // Usuario principal
        [Required]
        public Guid UserId1 { get; set; }
        [ForeignKey("UserId1")]
        public User User1 { get; set; } // Propiedad de navegación

        // Usuario relacionado
        [Required]
        public Guid UserId2 { get; set; }
        [ForeignKey("UserId2")]
        public User User2 { get; set; } // Propiedad de navegación


         // Nueva relación con DeviceUserRelation1
        public Guid? DeviceUserRelationId1 { get; set; }
        [ForeignKey("DeviceUserRelationId1")]
        public DeviceUserRelation DeviceUserRelation1 { get; set; }

        // Nueva relación con DeviceUserRelation2
        public Guid? DeviceUserRelationId2 { get; set; }
        [ForeignKey("DeviceUserRelationId2")]
        public DeviceUserRelation DeviceUserRelation2 { get; set; }
    }
}
