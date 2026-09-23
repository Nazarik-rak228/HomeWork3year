package org.example.creatfirtsapi.repository;


import org.example.creatfirtsapi.dto.CategoryDto;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface CategoryRepository extends JpaRepository<CategoryDto, Integer> {
    List<CategoryDto> findByCatNameContainingIgnoreCase(String catName);
}