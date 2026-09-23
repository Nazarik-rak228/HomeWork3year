package org.example.creatfirtsapi.dto;

import jakarta.persistence.*;


@Entity
@Table(name = "roles")
public class RoleDto {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private int id;
    @Column(name = "rol_name", nullable = false)
    private String rolName;

    public RoleDto() {

    }

    public RoleDto(int id, String rolName) {
        this.id = id;
        this.rolName = rolName;
    }

    public String getRolName() {
        return rolName;
    }

    public void setRolName(String rolName) {
        this.rolName = rolName;
    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }
}
