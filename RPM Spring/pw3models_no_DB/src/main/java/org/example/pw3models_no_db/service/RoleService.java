package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.model.RoleModel;

import java.util.List;

public interface RoleService {
    List<RoleModel> findAll();
    RoleModel addRole(RoleModel role);
    RoleModel updateRole(RoleModel role);
    void deleteRole(int id);
    List<RoleModel> findByRolName(String rolName);
    RoleModel findById(int id);
    RoleModel findByRolNameExact(String rolName);
}