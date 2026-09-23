package org.example.pw3models_no_db.service;

import org.example.pw3models_no_db.api.RoleApiClient;
import org.example.pw3models_no_db.model.RoleModel;
import org.springframework.stereotype.Service;

import java.util.List;

@Service
public class RoleServiceImpl implements RoleService {

    private final RoleApiClient roleApiClient;

    public RoleServiceImpl(RoleApiClient roleApiClient) {
        this.roleApiClient = roleApiClient;
    }

    @Override
    public List<RoleModel> findAll() {
        return roleApiClient.findAll();
    }

    @Override
    public RoleModel addRole(RoleModel role) {
        return roleApiClient.addRole(role);
    }

    @Override
    public RoleModel updateRole(RoleModel role) {
        roleApiClient.updateRole(role);
        return role;
    }

    @Override
    public void deleteRole(int id) {
        roleApiClient.deleteRole(id);
    }

    @Override
    public List<RoleModel> findByRolName(String rolName) {
        return roleApiClient.findByRolName(rolName);
    }

    @Override
    public RoleModel findById(int id) {
        return roleApiClient.findById(id);
    }

    @Override
    public RoleModel findByRolNameExact(String rolName) {
        return roleApiClient.findByRolNameExact(rolName);
    }
}