package org.example.pw3models_no_db.model;

import jakarta.validation.constraints.NotBlank;
import jakarta.validation.constraints.Pattern;

public class RoleModel {

    private int id;

    @NotBlank(message = "Название роли не может быть пустым")
    @Pattern(regexp = "^[A-ZА-Я][A-ZА-Я_]*$", message = "Роль должна быть в формате USER / MANAGER / ADMIN")
    private String rolName;

    public RoleModel(String rolName, int id) {
        this.rolName = rolName;
        this.id = id;
    }

    public RoleModel() {

    }

    public int getId() {
        return id;
    }

    public void setId(int id) {
        this.id = id;
    }

    public String getRolName() {
        return rolName;
    }

    public void setRolName(String rolName) {
        this.rolName = rolName;
    }
}