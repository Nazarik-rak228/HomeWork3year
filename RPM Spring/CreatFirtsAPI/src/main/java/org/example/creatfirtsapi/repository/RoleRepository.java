package org.example.creatfirtsapi.repository;

import org.example.creatfirtsapi.dto.RoleDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface RoleRepository extends JpaRepository<RoleDto, Integer> {
    List<RoleDto> findByRolNameContainingIgnoreCase(String rolName);
}
/*

@Repository
public class RoleRepository {

    private List<RoleModel> roles = new ArrayList<>();
    private AtomicInteger idCounter = new AtomicInteger(1);

    public List<RoleModel> findAll() {
        return new ArrayList<>(roles);
    }

    public RoleModel addRole(RoleModel role) {
        role.setId(idCounter.getAndIncrement());
        roles.add(role);
        return role;
    }

    public RoleModel updateRole(RoleModel role) {
        for (int i = 0; i < roles.size(); i++) {
            if (roles.get(i).getId() == role.getId()) {
                roles.set(i, role);
                return role;
            }
        }
        return null;
    }

    public void deleteRole(int id) {
        roles.removeIf(role -> role.getId() == id);
    }

    // Дополнительно: поиск по id
    public RoleModel findById(int id) {
        return roles.stream()
                .filter(role -> role.getId() == id)
                .findFirst()
                .orElse(null);
    }
    public List<RoleModel> findByRolName(String rolName) {
        return roles.stream()
                .filter(r -> r.getRolName() != null &&
                        r.getRolName().toLowerCase().contains(rolName.toLowerCase()))
                .toList();
    }
}*/
