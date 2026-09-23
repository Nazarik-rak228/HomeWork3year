package org.example.pw3models_no_db.api;

import org.example.pw3models_no_db.model.RoleModel;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.ParameterizedTypeReference;
import org.springframework.stereotype.Component;
import org.springframework.util.MultiValueMap;
import org.springframework.web.client.RestTemplate;

import java.util.List;

@Component
public class RoleApiClient extends BaseApiClient {

    public RoleApiClient(RestTemplate restTemplate,
                         @Value("${api.base.url}") String apiBaseUrl) {
        super(restTemplate, apiBaseUrl + "/api/role");
    }

    public List<RoleModel> findAll() {
        return getList("", new ParameterizedTypeReference<List<RoleModel>>() {});
    }

    public RoleModel findById(int id) {
        return getOne("/" + id, RoleModel.class);
    }

    public List<RoleModel> findByRolName(String rolName) {
        return findAll().stream()
                .filter(r -> r.getRolName() != null &&
                        r.getRolName().toLowerCase().contains(rolName.toLowerCase()))
                .toList();
    }

    public RoleModel findByRolNameExact(String rolName) {
        return findAll().stream()
                .filter(r -> r.getRolName() != null &&
                        r.getRolName().equalsIgnoreCase(rolName))
                .findFirst()
                .orElse(null);
    }

    public RoleModel addRole(RoleModel role) {
        MultiValueMap<String, String> params = form();
        add(params, "rolName", role.getRolName());
        return postForm("", params, RoleModel.class);
    }

    public void updateRole(RoleModel role) {
        MultiValueMap<String, String> params = form();
        add(params, "id", role.getId());
        add(params, "rolName", role.getRolName());
        putForm("", params);
    }

    public void deleteRole(int id) {
        delete("/" + id);
    }
}